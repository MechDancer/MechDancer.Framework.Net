using System;
using System.IO;
using System.Net.Sockets;
using MechDancer.Framework.Net.Dependency;
using MechDancer.Framework.Net.Remote.Resources;

namespace MechDancer.Framework.Net.Remote.Modules.TcpConnection {
	public sealed class DialogTcpServer : AbstractModule, IShortConnectionListener {
		private readonly Func<string, byte[], byte[]> _func;

		public DialogTcpServer(Func<string, byte[], byte[]> func)
			=> _func = func;

		public byte Interest => (byte) TcpCmd.Dialog;

		public void Process(string client, NetworkStream stream)
			=> stream.Say(_func(client, stream.Listen()));

		public override bool Equals(object obj) => obj is DialogTcpServer;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(DialogTcpServer).GetHashCode();
	}
}