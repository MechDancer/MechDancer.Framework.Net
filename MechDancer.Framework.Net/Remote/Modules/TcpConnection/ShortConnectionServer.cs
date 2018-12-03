using System.Collections.Generic;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Net.Remote.Protocol;
using MechDancer.Framework.Net.Remote.Resources;

namespace MechDancer.Framework.Net.Remote.Modules.TcpConnection {
	public sealed class ShortConnectionServer : AbstractDependent {
		private readonly Hook<ServerSockets> _servers;

		private readonly HashSet<IMailListener> _mailListeners
			= new HashSet<IMailListener>();

		private readonly Dictionary<byte, IShortConnectionListener> _connectListeners
			= new Dictionary<byte, IShortConnectionListener>();

		public ShortConnectionServer() => _servers = BuildDependency<ServerSockets>();

		public override bool Sync<T>(T dependency) {
			base.Sync(dependency);
			(dependency as IMailListener)?.Let(it => _mailListeners.Add(it));
			(dependency as IShortConnectionListener)?.Also(it => _connectListeners.Add(it.Interest, it));
			return false;
		}

		public void Invoke(int port = 0) {
			using (var stream = _servers.StrictField.Get(port).AcceptTcpClient().GetStream()) {
				var cmd    = stream.ListenCommand();
				var client = stream.ListenString();
				if ((TcpCmd) cmd == TcpCmd.Mail) {
					var payload = stream.ReadWithLength();
					foreach (var listener in _mailListeners)
						listener.Process(client, payload);
				} else if (_connectListeners.TryGetValue(cmd, out var listener)) {
					listener.Process(client, stream);
				}
			}
		}

		public override bool Equals(object obj) => obj is ShortConnectionServer;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(ShortConnectionServer).GetHashCode();
	}
}