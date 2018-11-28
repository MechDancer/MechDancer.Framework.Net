using System;
using System.Threading;
using MechDancer.Framework.Net.Dependency;
using MechDancer.Framework.Net.Remote.Protocol;
using static MechDancer.Framework.Net.Dependency.Functions;
using MechDancer.Framework.Net.Remote.Resources;

namespace MechDancer.Framework.Net.Remote.Modules.Multicast {
	public sealed class MulticastBroadcaster : AbstractModule {
		private readonly Lazy<Name>             _name; // 可以匿名发送组播
		private readonly Lazy<MulticastSockets> _sockets;

		public MulticastBroadcaster() {
			_name    = Maybe<Name>(Host);
			_sockets = Must<MulticastSockets>(Host);
		}

		public void Broadcast(byte cmd, byte[] payload = null) {
			var me = _name.Value?.Field;

			if (String.IsNullOrWhiteSpace(me)
			 && (cmd == (byte) UdpCmd.YellAck || cmd == (byte) UdpCmd.AddressAck)
			) return;

			var packet = new RemotePacket
				(sender: me ?? "",
				 command: cmd,
				 payload: payload ?? new byte[0]
				).Bytes;

			foreach (var socket in _sockets.Value.View.Values)
				socket.Broadcast(packet);
		}

		public override bool Equals(object obj) => obj is MulticastBroadcaster;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(MulticastBroadcaster).GetHashCode();
	}
}