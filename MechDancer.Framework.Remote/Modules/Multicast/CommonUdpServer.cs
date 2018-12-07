using System;
using System.Collections.Generic;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Dependency.UniqueComponent;
using MechDancer.Framework.Net.Protocol;
using MechDancer.Framework.Net.Resources;

namespace MechDancer.Framework.Net.Modules.Multicast {
	public sealed class CommonUdpServer : UniqueComponent<CommonUdpServer>,
	                                      IMulticastListener {
		private static readonly byte[] InterestSet = {(byte) UdpCmd.Common};

		private readonly Action<string, byte[]> _action;

		public CommonUdpServer(Action<string, byte[]> action) => _action = action;

		public IReadOnlyCollection<byte> Interest => InterestSet;

		public void Process(RemotePacket remotePacket) {
			var (name, _, payload) = remotePacket;
			_action(name, payload);
		}
	}
}