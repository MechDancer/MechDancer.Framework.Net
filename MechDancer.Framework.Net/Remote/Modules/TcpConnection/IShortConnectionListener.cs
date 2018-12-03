using System.Net.Sockets;
using MechDancer.Framework.Dependency;

namespace MechDancer.Framework.Net.Remote.Modules.TcpConnection {
	public interface IShortConnectionListener : IDependent {
		byte Interest { get; }

		void Process(string client, NetworkStream stream);
	}
}