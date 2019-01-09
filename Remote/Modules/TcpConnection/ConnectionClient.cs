using System.Net.Sockets;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Dependency.UniqueComponent;
using MechDancer.Framework.Net.Resources;

namespace MechDancer.Framework.Net.Modules.TcpConnection {
	public sealed class ConnectionClient : UniqueComponent<ConnectionClient>,
	                                       IDependent {
		private readonly UniqueDependency<Addresses>   _addresses;
		private readonly UniqueDependencies            _dependencies = new UniqueDependencies();
		private readonly UniqueDependency<PortMonitor> _monitor;
		private readonly UniqueDependency<Name>        _name;

		public ConnectionClient() {
			_name      = _dependencies.BuildDependency<Name>();
			_addresses = _dependencies.BuildDependency<Addresses>();
			_monitor   = _dependencies.BuildDependency<PortMonitor>();
		}

		public bool Sync(IComponent dependency) => _dependencies.Sync(dependency);

		public NetworkStream Connect(string name, byte cmd) {
			var address = _addresses.StrictField[name];
			if (address == null || (ushort) address.Port != address.Port) {
				_monitor.Field?.Ask(name);
				return null;
			}

			var socket = new TcpClient();
			try {
				socket.Connect(address);
				var stream = socket.GetStream();
				stream.Say(cmd);
				stream.Say(_name.Field?.Field ?? "");
			} catch (SocketException) {
				_addresses.StrictField.Remove(name);
				_monitor.Field?.Ask(name);
				return null;
			}

			return socket.GetStream();
		}
	}
}