using System.Collections.Generic;
using System.Net;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Dependency.UniqueComponent;
using MechDancer.Framework.Net.Modules.Multicast;
using MechDancer.Framework.Net.Protocol;
using MechDancer.Framework.Net.Resources;

namespace MechDancer.Framework.Net.Modules.TcpConnection {
	public sealed class PortBroadcaster : UniqueComponent<PortBroadcaster>,
	                                      IDependent,
	                                      IMulticastListener {
		private static readonly byte[] InterestSet = {(byte) UdpCmd.AddressAsk};

		private readonly UniqueDependencies _dependencies = new UniqueDependencies();

		private readonly UniqueDependency<MulticastBroadcaster> _broadcaster;
		private readonly UniqueDependency<Name>                 _name;
		private readonly UniqueDependency<ServerSockets>        _servers;

		public PortBroadcaster() {
			_name        = _dependencies.BuildDependency<Name>();
			_broadcaster = _dependencies.BuildDependency<MulticastBroadcaster>();
			_servers     = _dependencies.BuildDependency<ServerSockets>();
		}

		public bool Sync(IComponent dependency) => _dependencies.Sync(dependency);

		public IReadOnlyCollection<byte> Interest => InterestSet;

		public void Process(RemotePacket remotePacket) {
			if (remotePacket.Payload.Length      == 0
			 || remotePacket.Payload.GetString() != _name.StrictField.Field)
				return;

			var port = _servers.StrictField
			                   .Default
			                   .LocalEndpoint
			                   .Let(it => (IPEndPoint) it)
			                   .Port
			                   .Let(it => new[] {(byte) (it >> 8), (byte) it});

			_broadcaster.StrictField.Broadcast((byte) UdpCmd.AddressAck, port);
		}
	}
}