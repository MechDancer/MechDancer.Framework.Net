using System.Collections.Generic;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Net.Protocol;

namespace MechDancer.Framework.Net.Modules.Multicast {
	/// <inheritdoc />
	/// <summary>
	///     组播监听者
	/// </summary>
	public interface IMulticastListener : IComponent {
		IReadOnlyCollection<byte> Interest { get; }

		void Process(RemotePacket remotePacket);
	}
}