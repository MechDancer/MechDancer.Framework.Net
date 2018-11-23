using System;
using System.Threading;
using MechDancer.Framework.Net.Dependency;
using MechDancer.Framework.Net.Remote.Protocol;
using static MechDancer.Framework.Net.Dependency.Functions;
using MechDancer.Framework.Net.Remote.Resources;

namespace MechDancer.Framework.Net.Remote.Modules.Multicast {
	public class MulticastBroadcaster : AbstractModule {
		private readonly MaybeProperty<Name>    _name;
		private readonly Lazy<MulticastSockets> _sockets;
		private          long                   _serial;

		public MulticastBroadcaster() {
			_serial  = -1;
			_name    = Maybe<Name>(Host);
			_sockets = Must<MulticastSockets>(Host);
		}

		public void Broadcast(byte cmd, byte[] payload = null) {
			var packet = new RemotePacket
				(cmd,
				 _name[out var it] ? it.Field : "",
				 Interlocked.Increment(ref _serial),
				 payload ?? new byte[0]
				).Bytes;

			foreach (var socket in _sockets.Value.View.Values)
				socket.Broadcast(packet);
		}

		public override bool Equals(object obj) => obj is MulticastBroadcaster;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(MulticastBroadcaster).GetHashCode();
	}
}