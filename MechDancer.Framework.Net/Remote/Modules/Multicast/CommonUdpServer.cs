using System;
using System.Collections.Generic;
using MechDancer.Framework.Net.Dependency;
using MechDancer.Framework.Net.Remote.Protocol;
using MechDancer.Framework.Net.Remote.Resources;
using static MechDancer.Framework.Net.Dependency.Functions;

namespace MechDancer.Framework.Net.Remote.Modules.Multicast {
	public sealed class CommonUdpServer : AbstractModule, IMulticastListener {
		private readonly Lazy<MulticastBroadcaster> _broadcaster;
		private readonly Action<string, byte[]>     _action;

		public CommonUdpServer(Action<string, byte[]> action) {
			_action      = action;
			_broadcaster = Must<MulticastBroadcaster>(Dependencies);
		}

		public IReadOnlyCollection<byte> Interest => InterestSet;

		public void Process(RemotePacket remotePacket) {
			var (name, _, payload) = remotePacket;
			_action(name, payload);
		}

		public override bool Equals(object obj) => obj is CommonUdpServer;
		public override int  GetHashCode()      => Hash;

		private static readonly int           Hash        = typeof(CommonUdpServer).GetHashCode();
		private static readonly HashSet<byte> InterestSet = new HashSet<byte> {(byte) UdpCmd.Common};
	}
}