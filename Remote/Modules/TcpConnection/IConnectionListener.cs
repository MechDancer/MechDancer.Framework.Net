using System.Net.Sockets;
using MechDancer.Framework.Dependency;

namespace MechDancer.Framework.Net.Modules.TcpConnection {
	public interface IConnectionListener : IComponent {
		byte Interest { get; }

		void Process(string client, NetworkStream stream);
	}
}