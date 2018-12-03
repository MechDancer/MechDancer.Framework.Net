using MechDancer.Framework.Dependency;

namespace MechDancer.Framework.Net.Modules.TcpConnection {
	public interface IMailListener : IDependent {
		void Process(string sender, byte[] payload);
	}
}