using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using MechDancer.Framework.Net.Dependency;

namespace MechDancer.Framework.Net.Remote.Resources {
	public sealed class MulticastSockets : IResourceFactory<NetworkInterface, UdpMulticastClient> {
		private readonly ConcurrentDictionary<NetworkInterface, UdpMulticastClient> _core
			= new ConcurrentDictionary<NetworkInterface, UdpMulticastClient>();

		public IReadOnlyDictionary<NetworkInterface, UdpMulticastClient> View => _core;

		public readonly IPEndPoint         Address;
		public readonly UdpMulticastClient Default;

		public MulticastSockets(IPEndPoint address) {
			Address = address;
			Default = new UdpMulticastClient(address, null);
		}

		public UdpMulticastClient Get(NetworkInterface parameter) =>
			_core.GetOrAdd(parameter, network => new UdpMulticastClient(Address, network));

		public bool TryGet(NetworkInterface parameter, out UdpMulticastClient resource) {
			resource = Get(parameter);
			return true;
		}

		public UdpMulticastClient Temporary => new UdpMulticastClient(Address, null);

		public override bool Equals(object obj) => obj is MulticastSockets;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(MulticastSockets).GetHashCode();
	}
}