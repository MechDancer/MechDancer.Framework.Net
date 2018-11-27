using System;
using System.Net.Sockets;
using MechDancer.Framework.Net.Dependency;
using MechDancer.Framework.Net.Remote.Resources;
using static MechDancer.Framework.Net.Dependency.Functions;

namespace MechDancer.Framework.Net.Remote.Modules.TcpConnection {
	public sealed class ShortConnectionClient : AbstractModule {
		private readonly Lazy<Addresses>            _addresses;
		private readonly MaybeProperty<PortMonitor> _monitor;

		public ShortConnectionClient() {
			_addresses = Must<Addresses>(Host);
			_monitor   = Maybe<PortMonitor>(Host);
		}

		public TcpClient Connect(string name) {
			var address = _addresses.Value[name];
			if (address == null) return null;

			var socket = new TcpClient();
			try {
				socket.Connect(address);
			}
			catch (SocketException) {
				_addresses.Value.Remove(name);
				return null;
			}

			return socket;
		}

		public override bool Equals(object obj) => obj is ShortConnectionClient;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(ShortConnectionClient).GetHashCode();
	}
}