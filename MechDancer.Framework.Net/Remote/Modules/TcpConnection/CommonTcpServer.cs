using System;
using System.Net.Sockets;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Net.Remote.Resources;

namespace MechDancer.Framework.Net.Remote.Modules.TcpConnection {
	public sealed class CommonTcpServer : AbstractDependent, IShortConnectionListener {
		private readonly Action<string, NetworkStream> _func;

		public CommonTcpServer(Action<string, NetworkStream> func)
			=> _func = func;

		public byte Interest => (byte) TcpCmd.Common;

		public void Process(string client, NetworkStream stream)
			=> _func(client, stream);

		public override bool Equals(object obj) => obj is CommonTcpServer;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(CommonTcpServer).GetHashCode();
	}
}