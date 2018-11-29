using System;
using System.Collections.Generic;
using MechDancer.Framework.Net.Dependency;
using MechDancer.Framework.Net.Remote.Modules.Multicast;
using MechDancer.Framework.Net.Remote.Protocol;
using MechDancer.Framework.Net.Remote.Resources;
using static MechDancer.Framework.Net.Dependency.Functions;

namespace MechDancer.Framework.Net.Remote.Modules.TcpConnection {
	/// <summary>
	/// 	地址同步器
	/// </summary>
	/// <remarks>
	///		地址同步机制 1
	/// 	这个模块用于 TCP 连接的发起者
	/// 	依赖地址资源和组播收发功能
	/// 	将发起地址询问并更新地址资源
	/// </remarks>
	public sealed class PortMonitor : AbstractModule, IMulticastListener {
		private readonly Lazy<MulticastBroadcaster> _broadcaster;
		private readonly Lazy<Addresses>            _addresses;

		public PortMonitor() {
			_broadcaster = Must<MulticastBroadcaster>(Dependencies);
			_addresses   = Must<Addresses>(Dependencies);
		}

		public void Ask(string name)
			=> _broadcaster
			  .Value
			  .Broadcast((byte) UdpCmd.AddressAsk, name.GetBytes());

		public IReadOnlyCollection<byte> Interest => InterestSet;

		public void Process(RemotePacket remotePacket) {
			var (sender, _, payload) = remotePacket;

			if (!string.IsNullOrWhiteSpace(remotePacket.Sender))
				_addresses
				   .Value
				   .Update(sender, payload[0] << 8 | payload[1]);
		}

		public override bool Equals(object obj) => obj is PortMonitor;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(PortMonitor).GetHashCode();

		private static readonly HashSet<byte> InterestSet
			= new HashSet<byte> {(byte) UdpCmd.AddressAck};
	}
}