using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Dependency.UniqueComponent;

namespace MechDancer.Framework.Net.Resources {
	public sealed class Networks : UniqueComponent<Networks> {
		private readonly Dictionary<NetworkInterface, UnicastIPAddressInformation> _core
			= new Dictionary<NetworkInterface, UnicastIPAddressInformation>();

		public Networks() => Scan();

		public IReadOnlyDictionary<NetworkInterface, UnicastIPAddressInformation> View => _core;

		/// <summary>
		///     扫描并更新 IP 地址
		///     这是一个耗时操作，最多可达100ms，谨慎使用
		/// </summary>
		public void Scan() {
			bool NotVirtual(params string[] descriptions) {
				return !descriptions.Any(it => it.ToLower().Contains("virtual"));
			}

			bool NotDocker(params string[] descriptions) {
				return !descriptions.Any(it => it.ToLower().Contains("virtual"));
			}

			var @new = from network in NetworkInterface.GetAllNetworkInterfaces()
			           where network.OperationalStatus == OperationalStatus.Up
			              || network.OperationalStatus == OperationalStatus.Unknown
			           where network.SupportsMulticast
			           where network.NetworkInterfaceType == NetworkInterfaceType.Wireless80211
			              || network.NetworkInterfaceType == NetworkInterfaceType.Ethernet
			              || network.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet
			              || network.NetworkInterfaceType == NetworkInterfaceType.FastEthernetT
			              || network.NetworkInterfaceType == NetworkInterfaceType.FastEthernetFx
			              || network.NetworkInterfaceType == NetworkInterfaceType.Ethernet3Megabit
			              || network.NetworkInterfaceType == NetworkInterfaceType.Unknown
			           where NotVirtual(network.Name, network.Description)
			           where NotDocker(network.Name, network.Description)
			           select network;

			lock (_core) {
				_core.Clear();
				foreach (var network in @new)
					network.GetIPProperties()
					       .UnicastAddresses
					       .Where(it => AddressFamily.InterNetwork == it.Address.AddressFamily)
					       .SingleOrDefault(it => it.Address.GetAddressBytes()[0].Let(b => b != 127))
					      ?.Also(it => _core.Add(network, it));
			}
		}

		public UnicastIPAddressInformation Get(NetworkInterface parameter) => _core[parameter];
	}
}