using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MechDancer.Framework.Net.Dependency;
using MechDancer.Framework.Net.Remote.Protocol;
using MechDancer.Framework.Net.Remote.Resources;
using static MechDancer.Framework.Net.Dependency.Functions;

namespace MechDancer.Framework.Net.Remote.Modules.TcpConnection {
	public sealed class ShortConnectionServer : AbstractModule {
		private readonly Lazy<ServerSockets> _servers;

		private readonly HashSet<IMailListener> _mailListeners
			= new HashSet<IMailListener>();

		private readonly Dictionary<byte, IShortConnectionListener> _connectListeners
			= new Dictionary<byte, IShortConnectionListener>();

		public ShortConnectionServer() => _servers = Must<ServerSockets>(Dependencies);

		public override void Sync() {
			lock (_mailListeners) {
				_mailListeners.Clear();
				foreach (var listener in Dependencies
				                        .Value
				                        .Get<IMailListener>())
					_mailListeners.Add(listener);
			}

			lock (_connectListeners) {
				_connectListeners.Clear();
				foreach (var listener in Dependencies
				                        .Value
				                        .Get<IShortConnectionListener>()
				                        .Where(it => it.Interest != (byte) TcpCmd.Mail))
					_connectListeners.Add(listener.Interest, listener);
			}
		}

		public void Invoke(int port = 0) {
			using (var stream = _servers.Value.Get(port).AcceptTcpClient().GetStream()) {
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