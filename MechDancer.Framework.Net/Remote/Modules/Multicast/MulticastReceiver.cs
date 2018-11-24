using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MechDancer.Framework.Net.Dependency;
using MechDancer.Framework.Net.Remote.Protocol;
using MechDancer.Framework.Net.Remote.Resources;
using static MechDancer.Framework.Net.Dependency.Functions;

namespace MechDancer.Framework.Net.Remote.Modules.Multicast {
	public sealed class MulticastReceiver : AbstractModule {
		private readonly ThreadLocal<byte[]>         _buffer;    // 线程独立缓冲区
		private readonly Lazy<Name>                  _name;      // 过滤环路数据
		private readonly Lazy<MulticastSockets>      _socket;    // 接收套接字
		private readonly HashSet<IMulticastListener> _callbacks; // 处理回调

		public MulticastReceiver(uint bufferSize = 65536) {
			_buffer    = new ThreadLocal<byte[]>(() => new byte[bufferSize]);
			_name      = Must<Name>(Host);
			_socket    = Must<MulticastSockets>(Host);
			_callbacks = new HashSet<IMulticastListener>();
		}

		public override void Sync() {
			foreach (var listener in Host().Get<IMulticastListener>())
				_callbacks.Add(listener);
		}

		public void Invoke() {
			var length = _socket.Value.Default.Receive(_buffer.Value);
			var stream = new MemoryStream(_buffer.Value, 0, length, false);
			var sender = stream.ReadEnd();
			if (sender == _name.Value.Field) return;

			var packet = new RemotePacket
				(sender: sender,
				 command: (byte) stream.ReadByte(),
				 seqNumber: stream.ReadZigzag(false),
				 payload: stream.ReadRest());
			
			foreach (var listener in _callbacks.Where(it => it.Interest.Contains(packet.Command)))
				listener.Process(packet);
		}

		public override bool Equals(object obj) => obj is MulticastReceiver;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(MulticastReceiver).GetHashCode();
	}
}