using System;
using System.Net.Sockets;
using MechDancer.Framework.Net.Resources;

namespace MechDancer.Framework.Net.Modules.TcpConnection {
	public sealed class ConnectionListener : IConnectionListener {
		private readonly Action<string, NetworkStream> _func;

		public ConnectionListener(
			byte                          interest,
			Action<string, NetworkStream> func
		) {
			Interest = interest;
			_func    = func;
		}

		public byte Interest { get; }

		public void Process(string client, NetworkStream stream) => _func(client, stream);

		public override bool Equals(object obj)
			=> ReferenceEquals(this, obj) || obj is ConnectionListener others && others.Interest == Interest;

		public override int GetHashCode()
			=> typeof(ConnectionListener).GetHashCode();
	}

	public sealed class DialogListener : IConnectionListener {
		private readonly Func<string, byte[], byte[]> _func;

		public DialogListener(Func<string, byte[], byte[]> func)
			=> _func = func;

		public byte Interest => (byte) TcpCmd.Dialog;

		public void Process(string client, NetworkStream stream)
			=> stream.Say(_func(client, stream.Listen()));

		public override bool Equals(object obj) => obj is DialogListener;
		public override int  GetHashCode()      => typeof(DialogListener).GetHashCode();
	}

	public sealed class MailListener : IMailListener {
		private readonly Action<string, byte[]> _action;

		public MailListener(Action<string, byte[]> action)
			=> _action = action;

		public void Process(string sender, byte[] payload)
			=> _action(sender, payload);

		public override bool Equals(object obj) => ReferenceEquals(this, obj);
		public override int  GetHashCode()      => typeof(MailListener).GetHashCode();
	}
}