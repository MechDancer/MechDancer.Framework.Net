using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using MechDancer.Framework.Net.Dependency;
using static System.Net.NetworkInformation.NetworkInterfaceType;
using static System.Net.Sockets.AddressFamily;

namespace MechDancer.Framework.Net.Remote.Resources {
	public sealed class Networks : IResourceFactory<NetworkInterface, IPAddress> {
		private readonly Dictionary<NetworkInterface, IPAddress> _core
			= new Dictionary<NetworkInterface, IPAddress>();

		public IReadOnlyDictionary<NetworkInterface, IPAddress> View => _core;

		/// <summary>
		/// 	扫描并更新 IP 地址
		///    	这是一个耗时操作，最多可达100ms，谨慎使用
		/// </summary>
		public void Scan() {
			bool NotVirtual(string description) => !description.ToLower().Contains("virtual");

			var @new = from network in NetworkInterface.GetAllNetworkInterfaces()
			           where network.OperationalStatus == OperationalStatus.Up
			              || network.OperationalStatus == OperationalStatus.Unknown
			           where network.SupportsMulticast
			           where network.NetworkInterfaceType == Wireless80211
			              || network.NetworkInterfaceType == Ethernet
			              || network.NetworkInterfaceType == GigabitEthernet
			              || network.NetworkInterfaceType == FastEthernetT
			              || network.NetworkInterfaceType == FastEthernetFx
			              || network.NetworkInterfaceType == Ethernet3Megabit
			           where NotVirtual(network.Name)
			           where NotVirtual(network.Description)
			           select network;

			lock (_core) {
				_core.Clear();
				foreach (var network in @new)
					network.GetIPProperties()
					       .UnicastAddresses
					       .Select(it => it.Address)
					       .Where(it => InterNetwork == it.AddressFamily)
					       .SingleOrDefault(it => it.GetAddressBytes()[0].Let(b => 0 < b && b < 224 && b != 127))
					      ?.Also(it => _core.Add(network, it));
			}
		}

		public bool TryGet(NetworkInterface parameter, out IPAddress resource) =>
			_core.TryGetValue(parameter, out resource);

		public IPAddress Get(NetworkInterface parameter) => _core[parameter];

		public override bool Equals(object obj) => obj is Networks;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(Networks).GetHashCode();
	}
}