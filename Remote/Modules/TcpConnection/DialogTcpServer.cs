using System;
using System.Net.Sockets;
using MechDancer.Framework.Dependency.UniqueComponent;
using MechDancer.Framework.Net.Resources;

namespace MechDancer.Framework.Net.Modules.TcpConnection {
	using Func = Func<string, byte[], byte[]>;

	public sealed class DialogTcpServer : UniqueComponent<DialogTcpServer>,
										  IShortConnectionListener {
		private readonly Func _func;

		public DialogTcpServer(Func func) => _func = func;

		public byte Interest => (byte) TcpCmd.Dialog;

		public void Process(string client, NetworkStream stream)
			=> stream.Say(_func(client, stream.Listen()));
	}
}