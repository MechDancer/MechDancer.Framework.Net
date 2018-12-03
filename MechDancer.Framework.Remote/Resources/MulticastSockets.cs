using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using MechDancer.Framework.Dependency;

namespace MechDancer.Framework.Net.Resources {
	public sealed class MulticastSockets : IComponent {
		private static readonly int Hash = typeof(MulticastSockets).GetHashCode();

		private readonly ConcurrentDictionary<NetworkInterface, UdpMulticastClient> _core
			= new ConcurrentDictionary<NetworkInterface, UdpMulticastClient>();

		public readonly IPEndPoint         Address;
		public readonly UdpMulticastClient Default;

		public MulticastSockets(IPEndPoint address) {
			Address = address;
			Default = new UdpMulticastClient(address, null);
		}

		public IReadOnlyDictionary<NetworkInterface, UdpMulticastClient> View => _core;

		public UdpMulticastClient Temporary => new UdpMulticastClient(Address, null);

		public UdpMulticastClient Get(NetworkInterface parameter) {
			return _core.GetOrAdd(parameter, network => new UdpMulticastClient(Address, network));
		}

		public override bool Equals(object obj) {
			return obj is MulticastSockets;
		}

		public override int GetHashCode() {
			return Hash;
		}
	}
}