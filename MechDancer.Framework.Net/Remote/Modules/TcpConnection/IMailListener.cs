using MechDancer.Framework.Dependency;

namespace MechDancer.Framework.Net.Remote.Modules.TcpConnection {
	public interface IMailListener : IDependent {
		void Process(string sender, byte[] payload);
	}
}