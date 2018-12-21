using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using MechDancer.Framework.Dependency.UniqueComponent;

namespace MechDancer.Framework.Net.Resources {
	public sealed class MulticastSockets : UniqueComponent<MulticastSockets> {
		private readonly ConcurrentDictionary<IPAddress, UdpMulticastClient> _core
			= new ConcurrentDictionary<IPAddress, UdpMulticastClient>();

		public readonly IPEndPoint Address;

		public MulticastSockets(IPEndPoint address) => Address = address;

		public IReadOnlyDictionary<IPAddress, UdpMulticastClient> View => _core;

		public UdpMulticastClient Temporary => new UdpMulticastClient(Address, null);

		public UdpMulticastClient Get(IPAddress parameter)
			=> _core.GetOrAdd(parameter, network => new UdpMulticastClient(Address, network));
	}
}