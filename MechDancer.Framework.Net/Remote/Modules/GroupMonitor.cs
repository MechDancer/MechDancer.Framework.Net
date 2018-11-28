using System;
using System.Collections.Generic;
using MechDancer.Framework.Net.Dependency;
using MechDancer.Framework.Net.Remote.Modules.Multicast;
using MechDancer.Framework.Net.Remote.Protocol;
using MechDancer.Framework.Net.Remote.Resources;
using static MechDancer.Framework.Net.Dependency.Functions;

namespace MechDancer.Framework.Net.Remote.Modules {
	public sealed class GroupMonitor : AbstractModule, IMulticastListener {
		private readonly Action<string>             _detected;
		private readonly Lazy<Group>                _group;
		private readonly Lazy<MulticastBroadcaster> _broadcaster;

		public GroupMonitor(Action<string> detected = null) {
			_detected    = detected;
			_group       = Must<Group>(Host);
			_broadcaster = Must<MulticastBroadcaster>(Host);
		}

		public void Yell() => _broadcaster.Value.Broadcast((byte) UdpCmd.YellAsk);

		public IReadOnlyCollection<byte> Interest => InterestSet;

		public void Process(RemotePacket remotePacket) {
			var (name, cmd, _) = remotePacket;

			if (!string.IsNullOrWhiteSpace(name)) // 非匿名则保存名字
				if (!_group.Value.Update(name, DateTime.Now, out _))
					_detected?.Invoke(name);

			if (cmd == (byte) UdpCmd.YellAsk) // 回应询问
				_broadcaster.Value.Broadcast((byte) UdpCmd.YellAck);
		}

		public override bool Equals(object obj) => obj is GroupMonitor;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(GroupMonitor).GetHashCode();

		private static readonly HashSet<byte> InterestSet
			= new HashSet<byte> {(byte) UdpCmd.YellAsk, (byte) UdpCmd.YellAck};
	}
}