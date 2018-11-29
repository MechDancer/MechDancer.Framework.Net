using MechDancer.Framework.Net.Dependency;

namespace MechDancer.Framework.Net.Remote.Modules.TcpConnection {
	public interface IMailListener : IFunctionModule {
		void Process(string sender, byte[] payload);
	}
}