using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Dependency.UniqueComponent;
using MechDancer.Framework.Net.Protocol;
using MechDancer.Framework.Net.Resources;

namespace MechDancer.Framework.Net.Modules.Multicast {
	/// <summary>
	///     UDP 分片器
	/// </summary>
	public class PacketSlicer : UniqueComponent<PacketSlicer>,
	                            IMulticastListener {
		private static readonly byte[] InterestSet = {(byte) UdpCmd.PackageSlice};

		private readonly ConcurrentDictionary<(string, long), Buffer> _buffers;
		private readonly List<IMulticastListener>                     _listeners;

		private long _sequence;

		public PacketSlicer() {
			_buffers   = new ConcurrentDictionary<(string, long), Buffer>();
			_listeners = new List<IMulticastListener>();
		}

		public IReadOnlyCollection<byte> Interest => InterestSet;

		public void Process(RemotePacket remotePacket) {
			var (name, _, payload) = remotePacket;
			var   stream = new MemoryStream(payload);
			byte? cmd;
			if (payload[0] == 0) {
				stream.ReadByte();
				cmd = (byte) stream.ReadByte();
			} else {
				cmd = null;
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

		public bool Sync(IComponent dependency) {
			if (!(dependency is PacketSlicer))
				(dependency as IMulticastListener)?.Also(it => _listeners.Add(it));
			return false;
		}

		internal void Broadcast(byte cmd, byte[] payload, int size, Action<byte[]> output) {
			var stream = new MemoryStream(payload);
			var s      = Interlocked.Increment(ref _sequence).Zigzag(false);
			var index  = 0L; // 包序号

			while (stream.Position < payload.Length) {
				// 编码子包序号
				var i = index++.Zigzag(false);
				// 如果是最后一包，应该多长？
				var          last = (int) (stream.Length - stream.Position + 2 + s.Length + i.Length);
				byte[]       pack;
				MemoryStream outStream;
				if (last <= size) {
					pack      = new byte[last];
					outStream = new MemoryStream(pack);
					outStream.WriteByte(0);   // 空一位作为停止位
					outStream.WriteByte(cmd); // 保存实际指令
				} else {
					pack      = new byte[size];
					outStream = new MemoryStream(pack);
				}

				outStream.Write(s);
				outStream.Write(i);
				var length = outStream.Available();
				Array.Copy(payload, stream.Position,
				           pack, outStream.Position,
				           outStream.Available());
				stream.Position += length;

				output(pack);
			}
		}

		public void Refresh(TimeSpan timeout) {
			var now = DateTime.Now;
			foreach (var key in from buffer in _buffers
			                    where buffer.Value.By(now) > timeout
			                    select buffer.Key)
				_buffers.TryRemove(key, out _);
		}

		private class Buffer {
			private readonly List<Hook>            _list = new List<Hook>();
			private readonly Dictionary<int, Hook> _mark = new Dictionary<int, Hook>();

			private byte?    _command;
			private DateTime _time = DateTime.Now;
			private bool     Done => _command != null;

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