using System.Collections.Generic;
using System.Net;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Net.Remote.Modules.Multicast;
using MechDancer.Framework.Net.Remote.Protocol;
using MechDancer.Framework.Net.Remote.Resources;

namespace MechDancer.Framework.Net.Remote.Modules.TcpConnection {
	public sealed class PortBroadcaster : AbstractDependent, IMulticastListener {
		private readonly Hook<Name>                 _name;
		private readonly Hook<MulticastBroadcaster> _broadcaster;
		private readonly Hook<ServerSockets>        _servers;

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

		public override bool Equals(object obj) => obj is PortBroadcaster;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(PortBroadcaster).GetHashCode();

		private static readonly HashSet<byte> InterestSet
			= new HashSet<byte> {(byte) UdpCmd.AddressAsk};
	}
}