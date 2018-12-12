using MechDancer.Framework.Dependency;

namespace MechDancer.Framework.Net.Modules.TcpConnection {
	public interface IMailListener : IComponent {
		void Process(string sender, byte[] payload);
	}
}