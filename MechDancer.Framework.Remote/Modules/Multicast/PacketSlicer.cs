using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Net.Protocol;
using MechDancer.Framework.Net.Resources;

namespace MechDancer.Framework.Net.Modules.Multicast {
	public class PacketSlicer : AbstractDependent<PacketSlicer>,
	                            IMulticastListener {
		private static byte[] InterestSet = {(byte) UdpCmd.PackageSlice};

		// 发送

		private readonly int                                 _packetSize;
		private readonly ComponentHook<MulticastBroadcaster> _broadcaster;
		private          long                                _sequence;

		// 接收

		private readonly ConcurrentDictionary<(string, long), Buffer> _buffers;
		private readonly List<IMulticastListener>                     _listeners;

		public PacketSlicer(int packetSize = 0x4000) {
			_packetSize  = packetSize;
			_broadcaster = BuildDependency<MulticastBroadcaster>();
			_buffers     = new ConcurrentDictionary<(string, long), Buffer>();
			_listeners   = new List<IMulticastListener>();
		}

		public override bool Sync(IComponent dependency) {
			base.Sync(dependency);
			if (!(dependency is PacketSlicer))
				(dependency as IMulticastListener)?.Also(it => _listeners.Add(it));
			return false;
		}

		public IReadOnlyCollection<byte> Interest => InterestSet;

		public void Broadcast(byte cmd, byte[] payload) {
			var stream = new MemoryStream(payload);
			var s      = Interlocked.Increment(ref _sequence).Zigzag(false);
			var index  = 0L; // 包序号

			while (stream.Position < payload.Length) {
				// 编码子包序号
				var i = index++.Zigzag(false);
				// 如果是最后一包，应该多长？
				var    last = (int) (stream.Length - stream.Position + 2 + s.Length + i.Length);
				byte[] pack;
				if (last <= _packetSize) {
					pack = new byte[last];
					var outStream = new MemoryStream(pack);
					outStream.WriteByte(0);   // 空一位作为停止位
					outStream.WriteByte(cmd); // 保存实际指令
					outStream.Write(s);
					outStream.Write(i);
					stream.WriteTo(outStream);
				} else {
					pack = new byte[_packetSize];
					var outStream = new MemoryStream(pack);
					outStream.Write(s);
					outStream.Write(i);
					var length = _packetSize - outStream.Position;
					Array.Copy(payload, stream.Position,
					           pack, outStream.Position,
					           length);
					stream.Position += length;
				}

				_broadcaster.StrictField.Broadcast(InterestSet[0], pack);
			}
		}

		public void Process(RemotePacket remotePacket) {
			var (name, _, payload) = remotePacket;
			MemoryStream stream;
			byte?        cmd;
			if (payload[0] == 0) {
				stream = new MemoryStream(payload, 2, payload.Length - 2);
				cmd    = payload[1];
			} else {
				stream = new MemoryStream(payload);
				cmd    = null;
			}

			var subSeq = stream.ReadZigzag(false);
			var index  = stream.ReadZigzag(false);
			var rest   = payload.CopyRange((int) stream.Position);

			Tuple<byte, byte[]> result;
			if (index == 0 && cmd.HasValue) {
				result = new Tuple<byte, byte[]>(cmd.Value, rest);
			} else {
				var info = (name, subSeq);
				result = _buffers.GetOrAdd(info, new Buffer())
				                 .Put(cmd, (int) index, rest)
				                ?.Also(__ => _buffers.TryRemove(info, out _));
			}

			result?.Let(it => new RemotePacket(name, it.Item1, it.Item2))
			      ?.Also(it => {
				             foreach (
					             var listener
					             in from item in _listeners
					                where !item.Interest.Any() || item.Interest.Contains(it.Command)
					                select item
				             ) listener.Process(it);
			             });
		}

		private class Buffer {
			private DateTime _time = DateTime.Now;

			private readonly List<Hook>            _list = new List<Hook>();
			private readonly Dictionary<int, Hook> _mark = new Dictionary<int, Hook>();

			private byte? _command;
			private bool  Done => _command != null;

			public TimeSpan By(in DateTime now) => now - _time;

			public Tuple<byte, byte[]> Put(byte? cmd, int index, byte[] payload) {
				lock (_list) {
					if (Done) {
						_mark[index].Ptr = payload;
						_mark.Remove(index);
					} else {
						_command = cmd;

						for (var i = _list.Count; i < index; ++i) {
							var hook = new Hook {Ptr = null};
							_mark[i] = hook;
							_list.Add(hook);
						}

						if (_list.Count != index) {
							_mark[index].Ptr = payload;
							_mark.Remove(index);
						} else {
							_list.Add(new Hook {Ptr = payload});
						}
					}
				}

				if (Done && _mark.None()) {
					Debug.Assert(_command != null, nameof(_command) + " != null");
					return new Tuple<byte, byte[]>
						(_command.Value,
						 new MemoryStream(_list.Sum(it => it.Ptr.Length))
							.Also(it => {
								      foreach (var hook in _list) it.Write(hook.Ptr);
							      })
							.GetBuffer());
				}

				_time = DateTime.Now;
				return null;
			}

			private class Hook {
				public byte[] Ptr;
			}
		}
	}
}