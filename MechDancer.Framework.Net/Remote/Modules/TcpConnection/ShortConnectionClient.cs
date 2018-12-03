using System.Net.Sockets;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Net.Remote.Resources;

namespace MechDancer.Framework.Net.Remote.Modules.TcpConnection {
	public sealed class ShortConnectionClient : AbstractDependent {
		private readonly Hook<Name>        _name;
		private readonly Hook<Addresses>   _addresses;
		private readonly Hook<PortMonitor> _monitor;

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

		public override bool Equals(object obj) => obj is ShortConnectionClient;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(ShortConnectionClient).GetHashCode();
	}
}