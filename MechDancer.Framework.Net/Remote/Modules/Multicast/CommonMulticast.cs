using System;
using System.Collections.Generic;
using MechDancer.Framework.Net.Dependency;
using MechDancer.Framework.Net.Remote.Protocol;
using MechDancer.Framework.Net.Remote.Resources;
using static MechDancer.Framework.Net.Dependency.Functions;

namespace MechDancer.Framework.Net.Remote.Modules.Multicast {
	public sealed class CommonMulticast : AbstractModule, IMulticastListener {
		private readonly Lazy<MulticastBroadcaster> _broadcaster;
		private readonly Action<string, byte[]>     _action;

		public CommonMulticast(Action<string, byte[]> action) {
			_action      = action;
			_broadcaster = Must<MulticastBroadcaster>(Host);
		}

		public void Broadcast(byte[] payload) =>
			_broadcaster.Value.Broadcast((byte) UdpCmd.Common, payload);

		public IReadOnlyCollection<byte> Interest => InterestSet;

		public void Process(RemotePacket remotePacket) {
			var (name, _, payload) = remotePacket;
			_action(name, payload);
		}

		public override bool Equals(object obj) => obj is CommonMulticast;
		public override int  GetHashCode()      => Hash;

		private static readonly int           Hash        = typeof(CommonMulticast).GetHashCode();
		private static readonly HashSet<byte> InterestSet = new HashSet<byte> {(byte) UdpCmd.Common};
	}
}