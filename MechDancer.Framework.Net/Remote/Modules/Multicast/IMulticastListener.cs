using System.Collections.Generic;
using MechDancer.Framework.Net.Dependency;
using MechDancer.Framework.Net.Remote.Protocol;

namespace MechDancer.Framework.Net.Remote.Modules.Multicast {
	/// <summary>
	/// 	组播监听者
	/// </summary>
	public interface IMulticastListener : IFunctionModule {
		IReadOnlyCollection<byte> Interest { get; }

		void Process(RemotePacket remotePacket);
	}
}