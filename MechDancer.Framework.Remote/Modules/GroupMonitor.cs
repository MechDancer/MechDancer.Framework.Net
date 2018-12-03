using System;
using System.Collections.Generic;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Net.Modules.Multicast;
using MechDancer.Framework.Net.Protocol;
using MechDancer.Framework.Net.Resources;

namespace MechDancer.Framework.Net.Modules {
	public sealed class GroupMonitor : AbstractDependent<GroupMonitor>,
	                                   IMulticastListener {
		private static readonly byte[] InterestSet = {(byte) UdpCmd.YellAsk, (byte) UdpCmd.YellAck};

		private readonly ComponentHook<MulticastBroadcaster> _broadcaster;
		private readonly ComponentHook<Group>                _group;
		private readonly Action<string>                      _detected;

		public GroupMonitor(Action<string> detected = null) {
			_detected    = detected;
			_group       = BuildDependency<Group>();
			_broadcaster = BuildDependency<MulticastBroadcaster>();
		}

		public IReadOnlyCollection<byte> Interest => InterestSet;

		public void Process(RemotePacket remotePacket) {
			var (name, cmd, _) = remotePacket;

			if (!string.IsNullOrWhiteSpace(name)) // 非匿名则保存名字
				if (!_group.StrictField.Update(name, DateTime.Now, out _))
					_detected?.Invoke(name);

			if (cmd == (byte) UdpCmd.YellAsk) // 回应询问
				_broadcaster.Field?.Broadcast((byte) UdpCmd.YellAck);
		}

		public void Yell() => _broadcaster.StrictField.Broadcast((byte) UdpCmd.YellAsk);
	}
}