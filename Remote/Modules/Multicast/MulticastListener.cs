using System;
using System.Collections.Generic;
using MechDancer.Framework.Net.Protocol;
using MechDancer.Framework.Net.Resources;

namespace MechDancer.Framework.Net.Modules.Multicast {
	/// <summary>
	///     组播监听者的便捷实现
	/// </summary>
	public class MulticastListener : IMulticastListener {
		private readonly Action<RemotePacket> _callback;

		public MulticastListener(
			IReadOnlyCollection<byte> interest,
			Action<RemotePacket>      callback
		) {
			Interest  = interest;
			_callback = callback;
		}

		public IReadOnlyCollection<byte> Interest { get; }

		public void Process(RemotePacket remotePacket) {
			_callback(remotePacket);
		}

		public static MulticastListener CommonUdpListener(Action<string, byte[]> action) {
			return new MulticastListener(new[] {(byte) UdpCmd.Common},
										 pack => action(pack.Sender, pack.Payload));
		}
	}
}