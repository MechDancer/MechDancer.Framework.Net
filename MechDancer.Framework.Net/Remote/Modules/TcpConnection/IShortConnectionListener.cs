using System.Collections.Generic;
using System.Net.Sockets;
using MechDancer.Framework.Net.Dependency;

namespace MechDancer.Framework.Net.Remote.Modules.TcpConnection {
	public interface IShortConnectionListener : IFunctionModule {
		IReadOnlyCollection<byte> Interest { get; }

		void Process(NetworkStream stream);
	}
}