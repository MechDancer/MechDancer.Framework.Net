using System;
using System.IO;
using System.Net.Sockets;
using MechDancer.Framework.Net.Dependency;
using MechDancer.Framework.Net.Remote.Protocol;
using MechDancer.Framework.Net.Remote.Resources;
using static MechDancer.Framework.Net.Dependency.Functions;

namespace MechDancer.Framework.Net.Remote.Modules.TcpConnection {
	public sealed class ShortConnectionClient : AbstractModule {
		private readonly Lazy<Name>        _name;
		private readonly Lazy<Addresses>   _addresses;
		private readonly Lazy<PortMonitor> _monitor;

		public ShortConnectionClient() {
			_name      = Must<Name>(Dependencies);
			_addresses = Must<Addresses>(Dependencies);
			_monitor   = Maybe<PortMonitor>(Dependencies);
		}

		public NetworkStream Connect(string name, byte cmd) {
			var address = _addresses.Value[name];
			if (address == null) {
				_monitor.Value?.Ask(name);
				return null;
			}

			var socket = new TcpClient();
			try {
				socket.Connect(address);
				var stream = socket.GetStream();
				stream.Say(cmd);
				stream.Say(_name.Value.Field);
			}
			catch (SocketException) {
				_addresses.Value.Remove(name);
				_monitor.Value?.Ask(name);
				return null;
			}

			return socket.GetStream();
		}

		public override bool Equals(object obj) => obj is ShortConnectionClient;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(ShortConnectionClient).GetHashCode();
	}
}