using System;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Net.Remote.Protocol;
using MechDancer.Framework.Net.Remote.Resources;

namespace MechDancer.Framework.Net.Remote.Modules.Multicast {
	/// <summary>
	/// 	组播发布者
	/// </summary>
	public sealed class MulticastBroadcaster : AbstractDependent {
		private readonly Hook<Name>             _name; // 可以匿名发送组播
		private readonly Hook<MulticastSockets> _sockets;

		public MulticastBroadcaster() {
			_name    = BuildDependency<Name>();
			_sockets = BuildDependency<MulticastSockets>();
		}

		public void Broadcast(byte cmd, byte[] payload = null) {
			var me = _name.Field?.Field;

			if (String.IsNullOrWhiteSpace(me)
			 && (cmd == (byte) UdpCmd.YellAck || cmd == (byte) UdpCmd.AddressAck)
			) return;

			var packet = new RemotePacket
				(sender: me ?? "",
				 command: cmd,
				 payload: payload ?? new byte[0]
				).Bytes;

			foreach (var socket in _sockets.StrictField.View.Values)
				socket.Broadcast(packet);
		}

		public override bool Equals(object obj) => obj is MulticastBroadcaster;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(MulticastBroadcaster).GetHashCode();
	}
}