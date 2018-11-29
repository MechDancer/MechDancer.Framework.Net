using System;
using System.Collections.Generic;
using System.Net;
using MechDancer.Framework.Net.Dependency;
using MechDancer.Framework.Net.Remote.Modules.Multicast;
using MechDancer.Framework.Net.Remote.Protocol;
using MechDancer.Framework.Net.Remote.Resources;
using static MechDancer.Framework.Net.Dependency.Functions;

namespace MechDancer.Framework.Net.Remote.Modules.TcpConnection {
	public sealed class PortBroadcaster : AbstractModule, IMulticastListener {
		private readonly Lazy<Name>                 _name;
		private readonly Lazy<MulticastBroadcaster> _broadcaster;
		private readonly Lazy<ServerSockets>        _servers;

		public PortBroadcaster() {
			_name        = Must<Name>(Dependencies);
			_broadcaster = Must<MulticastBroadcaster>(Dependencies);
			_servers     = Must<ServerSockets>(Dependencies);
		}

		public IReadOnlyCollection<byte> Interest => InterestSet;

		public void Process(RemotePacket remotePacket) {
			if (remotePacket.Payload.GetString() != _name.Value.Field) return;

			var port = (IPEndPoint) _servers.Value.Default.LocalEndpoint;
			_broadcaster.Value.Broadcast
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