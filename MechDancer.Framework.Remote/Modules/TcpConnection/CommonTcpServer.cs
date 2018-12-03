using System;
using System.Net.Sockets;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Net.Resources;

namespace MechDancer.Framework.Net.Modules.TcpConnection {
	using Func = Action<string, NetworkStream>;

	public sealed class CommonTcpServer : AbstractDependent<CommonTcpServer>,
	                                      IShortConnectionListener {
		private readonly Func _func;

		public CommonTcpServer(Func func) => _func = func;

		public byte Interest => (byte) TcpCmd.Common;

		public void Process(string client, NetworkStream stream) => _func(client, stream);
	}
}