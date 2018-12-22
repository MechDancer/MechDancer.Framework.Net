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

		public UdpMulticastClient this[NetworkInterface @interface, IPAddress address] {
			get {
				Default.Bind(address);
				return _core.GetOrAdd(@interface, new UdpMulticastClient(Address, @interface));
			}
		}

		public void Open(NetworkInterface @interface, IPAddress address) {
			Default.Bind(address);
			_core.TryAdd(@interface, new UdpMulticastClient(Address, @interface));
		}
	}
}