using MechDancer.Framework.Dependency;
using MechDancer.Framework.Net.Protocol;
using MechDancer.Framework.Net.Resources;

namespace MechDancer.Framework.Net.Modules.Multicast {
	/// <summary>
	///     组播发布者
	/// </summary>
	public sealed class MulticastBroadcaster : AbstractDependent<MulticastBroadcaster> {
		private readonly ComponentHook<Name>             _name; // 可以匿名发送组播
		private readonly ComponentHook<MulticastSockets> _sockets;

		public MulticastBroadcaster() {
			_name    = BuildDependency<Name>();
			_sockets = BuildDependency<MulticastSockets>();
		}

		public void Broadcast(byte cmd, byte[] payload = null) {
			var me = _name.Field?.Field;

			if (string.IsNullOrWhiteSpace(me)
			 && (cmd == (byte) UdpCmd.YellAck || cmd == (byte) UdpCmd.AddressAck)
			) return;

			var packet = new RemotePacket
				(me ?? "",
				 cmd,
				 payload ?? new byte[0]
				).Bytes;

			foreach (var socket in _sockets.StrictField.View.Values)
				socket.Broadcast(packet);
		}
	}
}