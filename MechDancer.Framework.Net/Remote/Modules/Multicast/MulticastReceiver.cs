using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MechDancer.Framework.Net.Dependency;
using MechDancer.Framework.Net.Remote.Protocol;
using MechDancer.Framework.Net.Remote.Resources;
using static MechDancer.Framework.Net.Dependency.Functions;

namespace MechDancer.Framework.Net.Remote.Modules.Multicast {
	public sealed class MulticastReceiver : AbstractModule {
		private readonly ThreadLocal<byte[]>         _buffer;
		private readonly Lazy<Name>                  _name;
		private readonly Lazy<MulticastSockets>      _socket;
		private readonly HashSet<IMulticastListener> _callbacks;

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
			var pack = _socket
			          .Value
			          .Default
			          .ReceiveActual(_buffer.Value)
			          .Let(data => new RemotePacket(data));

			if (pack.Sender == _name.Value.Field) return;

			foreach (var listener in _callbacks.Where(it => it.Interest.Contains(pack.Command)))
				listener.Process(pack);
		}

		public override bool Equals(object obj) => obj is MulticastReceiver;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(MulticastReceiver).GetHashCode();
	}
}