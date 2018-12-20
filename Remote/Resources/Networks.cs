using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Dependency.UniqueComponent;
using Extensions = MechDancer.Common.Extensions;

namespace MechDancer.Framework.Net.Resources {
	/// <summary>
	///     网络接口资源
	/// </summary>
	/// <remarks>
	///     保存网络接口到其主要IPV4单播地址的映射
	/// </remarks>
	public sealed class Networks : UniqueComponent<Networks> {
		private readonly Dictionary<NetworkInterface, UnicastIPAddressInformation> _core
			= new Dictionary<NetworkInterface, UnicastIPAddressInformation>();

		private readonly ReaderWriterLock _lock = new ReaderWriterLock();

		/// <summary>
		///     构造器
		/// </summary>
		/// <remarks>
		///     构造时自动扫描一次网络
		/// </remarks>
		public Networks() => Scan();

		/// <summary>
		///     获取映射的副本
		/// </summary>
		public IReadOnlyDictionary<NetworkInterface, UnicastIPAddressInformation> View
			=> Extensions.Read(_lock, () => new Dictionary<NetworkInterface, UnicastIPAddressInformation>(_core));

		/// <summary>
		///     扫描并更新 IP 地址
		///     这是一个耗时操作，最多可达100ms，谨慎使用
		/// </summary>
		public void Scan() {
			bool NotDocker(params string[] descriptions) => !descriptions.Any(it => it.ToLower().Contains("docker"));

			// 筛选：状态；支持组播；是WiFi、以太网或未知类型；非docker创建的虚拟网卡；
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
			           where NotDocker(network.Name, network.Description)
			           select network;

			Extensions.Write(_lock,
			                 () => {
				                 _core.Clear();
				                 foreach (var network in @new)
					                 network.GetIPProperties()
					                        .UnicastAddresses
					                        .Where(it => AddressFamily.InterNetwork == it.Address.AddressFamily)
					                        .SingleOrDefault(it => it.Address.GetAddressBytes()[0]
					                                                 .Let(b => b != 127))
					                       ?.Also(it => _core.Add(network, it));
			                 });
		}
	}
}