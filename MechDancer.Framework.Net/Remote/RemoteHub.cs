using System.Net;
using System.Net.Sockets;
using MechDancer.Framework.Net.Dependency;
using MechDancer.Framework.Net.Remote.Modules;
using MechDancer.Framework.Net.Remote.Modules.Multicast;
using MechDancer.Framework.Net.Remote.Modules.TcpConnection;
using MechDancer.Framework.Net.Remote.Protocol;
using MechDancer.Framework.Net.Remote.Resources;
using static MechDancer.Framework.Net.Dependency.Functions;

// ReSharper disable RedundantAssignment

namespace MechDancer.Framework.Net.Remote {
	public sealed class RemoteHub {
		private readonly Group        _group;
		private readonly GroupMonitor _monitor;

		private readonly Networks             _networks;
		private readonly MulticastSockets     _sockets;
		private readonly MulticastBroadcaster _broadcaster;
		private readonly MulticastReceiver    _receiver;
		private readonly CommonMulticast      _commonMulticast;

		private readonly Addresses       _addresses;
		private readonly ServerSockets   _servers;
		private readonly PortBroadcaster _synchronizer1;
		private readonly PortMonitor     _synchronizer2;

		private readonly ShortConnectionClient _client;
		private readonly ShortConnectionServer _server;

		public readonly DynamicScope Hub;

		public RemoteHub(string name) {
			_group           = new Group();
			_monitor         = new GroupMonitor();
			_networks        = new Networks();
			_sockets         = new MulticastSockets(Address);
			_broadcaster     = new MulticastBroadcaster();
			_receiver        = new MulticastReceiver();
			_commonMulticast = new CommonMulticast(null);
			_addresses       = new Addresses();
			_servers         = new ServerSockets();
			_synchronizer1   = new PortBroadcaster();
			_synchronizer2   = new PortMonitor();
			_client          = new ShortConnectionClient();
			_server          = new ShortConnectionServer();

			Hub = Scope(@this => {
				            @this += new Name(name);

				            @this += _group;
				            @this += _monitor;

				            @this += _networks;
				            @this += _sockets;
				            @this += _broadcaster;
				            @this += _receiver;
				            @this += _commonMulticast;

				            @this += _addresses;
				            @this += _servers;
				            @this += _synchronizer1;
				            @this += _synchronizer2;

				            @this += _client;
				            @this += _server;
			            });
		}

		public void OpenAllNetworks() {
			foreach (var network in _networks.View.Keys)
				_sockets.Get(network);
		}
		
		public RemotePacket Invoke() => _receiver.Invoke();
		public void         Accept() => _server.Invoke();

		public NetworkStream Connect(string name, byte cmd) => _client.Connect(name, cmd);

		private static readonly IPEndPoint Address
			= new IPEndPoint(IPAddress.Parse("233.33.33.33"), 23333);
	}
}