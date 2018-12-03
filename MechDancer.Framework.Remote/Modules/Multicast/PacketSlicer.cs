using System;
using System.Collections.Generic;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Net.Protocol;

namespace MechDancer.Framework.Net.Modules.Multicast {
	public class PacketSlicer : AbstractDependent<PacketSlicer>, IMulticastListener {
		private readonly int _packetSize;

		public PacketSlicer(int packetSize = 0x4000) => _packetSize = packetSize;

		public IReadOnlyCollection<byte> Interest                           { get; }
		public void                      Process(RemotePacket remotePacket) => throw new NotImplementedException();
	}
}