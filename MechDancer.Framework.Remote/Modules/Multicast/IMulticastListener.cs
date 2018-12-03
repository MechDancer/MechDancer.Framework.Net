using System.Collections.Generic;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Net.Protocol;

namespace MechDancer.Framework.Net.Modules.Multicast {
	/// <summary>
	///     组播监听者
	/// </summary>
	public interface IMulticastListener : IDependent {
		IReadOnlyCollection<byte> Interest { get; }

		void Process(RemotePacket remotePacket);
	}
}