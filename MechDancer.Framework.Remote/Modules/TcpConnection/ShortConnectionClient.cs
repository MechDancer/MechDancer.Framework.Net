using System.Net.Sockets;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Net.Resources;

namespace MechDancer.Framework.Net.Modules.TcpConnection {
	public sealed class ShortConnectionClient : AbstractDependent<ShortConnectionClient> {
		private readonly ComponentHook<Addresses>   _addresses;
		private readonly ComponentHook<PortMonitor> _monitor;
		private readonly ComponentHook<Name>        _name;

		public ShortConnectionClient() {
			_name      = BuildDependency<Name>();
			_addresses = BuildDependency<Addresses>();
			_monitor   = BuildDependency<PortMonitor>();
		}

		public NetworkStream Connect(string name, byte cmd) {
			var address = _addresses.StrictField[name];
			if (address == null) {
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