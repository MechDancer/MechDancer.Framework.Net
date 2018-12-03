using System.Collections.Generic;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Net.Modules.Multicast;
using MechDancer.Framework.Net.Protocol;
using MechDancer.Framework.Net.Resources;

namespace MechDancer.Framework.Net.Modules.TcpConnection {
	/// <summary>
	///     地址同步器
	/// </summary>
	/// <remarks>
	///     地址同步机制 1
	///     这个模块用于 TCP 连接的发起者
	///     依赖地址资源和组播收发功能
	///     将发起地址询问并更新地址资源
	/// </remarks>
	public sealed class PortMonitor : AbstractDependent<PortMonitor>,
	                                  IMulticastListener {
		private static readonly HashSet<byte> InterestSet
			= new HashSet<byte> {(byte) UdpCmd.AddressAck};

		private readonly ComponentHook<Addresses>            _addresses;
		private readonly ComponentHook<MulticastBroadcaster> _broadcaster;

		public PortMonitor() {
			_broadcaster = BuildDependency<MulticastBroadcaster>();
			_addresses   = BuildDependency<Addresses>();
		}

		public IReadOnlyCollection<byte> Interest => InterestSet;

		public void Process(RemotePacket remotePacket) {
			var (sender, _, payload) = remotePacket;

			if (!string.IsNullOrWhiteSpace(remotePacket.Sender))
				_addresses
				   .StrictField
				   .Update(sender, (payload[0] << 8) | payload[1]);
		}

		public void Ask(string name)
			=> _broadcaster
			  .StrictField
			  .Broadcast((byte) UdpCmd.AddressAsk, name.GetBytes());
	}
}