using System.Collections.Generic;
using System.Net;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Net.Modules.Multicast;
using MechDancer.Framework.Net.Protocol;
using MechDancer.Framework.Net.Resources;

namespace MechDancer.Framework.Net.Modules.TcpConnection {
	public sealed class PortBroadcaster : AbstractDependent<PortBroadcaster>,
	                                      IMulticastListener {
		private static readonly byte[] InterestSet = {(byte) UdpCmd.AddressAsk};

		private readonly ComponentHook<MulticastBroadcaster> _broadcaster;
		private readonly ComponentHook<Name>                 _name;
		private readonly ComponentHook<ServerSockets>        _servers;

		public PortBroadcaster() {
			_name        = BuildDependency<Name>();
			_broadcaster = BuildDependency<MulticastBroadcaster>();
			_servers     = BuildDependency<ServerSockets>();
		}

		public IReadOnlyCollection<byte> Interest => InterestSet;

		public void Process(RemotePacket remotePacket) {
			if (remotePacket.Payload.GetString() != _name.StrictField.Field) return;

			var port = (IPEndPoint) _servers.StrictField.Default.LocalEndpoint;
			_broadcaster.StrictField.Broadcast
				((byte) UdpCmd.AddressAck,
				 new[] {(byte) (port.Port >> 8), (byte) port.Port});
		}
	}
}