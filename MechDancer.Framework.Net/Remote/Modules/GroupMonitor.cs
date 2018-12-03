using System;
using System.Collections.Generic;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Net.Remote.Modules.Multicast;
using MechDancer.Framework.Net.Remote.Protocol;
using MechDancer.Framework.Net.Remote.Resources;

namespace MechDancer.Framework.Net.Remote.Modules {
	public sealed class GroupMonitor : AbstractDependent, IMulticastListener {
		private readonly Action<string>             _detected;
		private readonly Hook<Group>                _group;
		private readonly Hook<MulticastBroadcaster> _broadcaster;

		public GroupMonitor(Action<string> detected = null) {
			_detected    = detected;
			_group       = BuildDependency<Group>();
			_broadcaster = BuildDependency<MulticastBroadcaster>();
		}

		public void Yell() => _broadcaster.StrictField.Broadcast((byte) UdpCmd.YellAsk);

		public IReadOnlyCollection<byte> Interest => InterestSet;

		public void Process(RemotePacket remotePacket) {
			var (name, cmd, _) = remotePacket;

			if (!string.IsNullOrWhiteSpace(name)) // 非匿名则保存名字
				if (!_group.StrictField.Update(name, DateTime.Now, out _))
					_detected?.Invoke(name);

			if (cmd == (byte) UdpCmd.YellAsk) // 回应询问
				_broadcaster.Field?.Broadcast((byte) UdpCmd.YellAck);
		}

		public override bool Equals(object obj) => obj is GroupMonitor;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(GroupMonitor).GetHashCode();

		private static readonly HashSet<byte> InterestSet
			= new HashSet<byte> {(byte) UdpCmd.YellAsk, (byte) UdpCmd.YellAck};
	}
}