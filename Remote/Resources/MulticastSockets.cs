using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using MechDancer.Common;
using MechDancer.Framework.Dependency.UniqueComponent;

namespace MechDancer.Framework.Net.Resources {
	public sealed class MulticastSockets : UniqueComponent<MulticastSockets> {
		private readonly ConcurrentDictionary<NetworkInterface, UdpMulticastClient> _core
			= new ConcurrentDictionary<NetworkInterface, UdpMulticastClient>();

		public readonly UdpMulticastClient Default;

		public readonly IPEndPoint Group;

		public MulticastSockets(IPEndPoint group) {
			Group   = group;
			Default = new UdpMulticastClient(group, null);
		}

		public IReadOnlyDictionary<NetworkInterface, UdpMulticastClient> View => _core;

		public UdpMulticastClient Temporary => new UdpMulticastClient(Group, null);

		/// <summary>
		///     指定可以从一个本地IP地址接收
		/// </summary>
		/// <param name="address">本地单播IP地址</param>
		public void Bind(IPAddress address)
			=> Default.Bind(address);

		/// <summary>
		///     指定可以从一个本地网络接口发送和接收
		/// </summary>
		/// <param name="interface">本地网络接口</param>
		public void Open(NetworkInterface @interface) {
			@interface.GetIPProperties()
					  .UnicastAddresses
					  .Select(it => it.Address)
					  .FirstOrDefault(it => it.AddressFamily == AddressFamily.InterNetwork)
					 ?.Also(Default.Bind);
			_core.TryAdd(@interface, new UdpMulticastClient(Group, @interface));
		}
	}
}