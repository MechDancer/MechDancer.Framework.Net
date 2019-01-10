using System.Collections.Generic;
using MechDancer.Common;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Dependency.UniqueComponent;
using MechDancer.Framework.Net.Protocol;
using MechDancer.Framework.Net.Resources;

namespace MechDancer.Framework.Net.Modules.TcpConnection {
	public sealed class ConnectionServer : UniqueComponent<ConnectionServer>,
										   IDependent {
		private readonly Dictionary<byte, IConnectionListener> _connectListeners
			= new Dictionary<byte, IConnectionListener>();

		private readonly UniqueDependencies _dependencies = new UniqueDependencies();

		private readonly HashSet<IMailListener> _mailListeners
			= new HashSet<IMailListener>();

		private readonly UniqueDependency<ServerSockets> _servers;

		public ConnectionServer() => _servers = _dependencies.BuildDependency<ServerSockets>();

		public bool Sync(IComponent dependency) {
			_dependencies.Sync(dependency);
			(dependency as IMailListener)?.Let(it => _mailListeners.Add(it));
			(dependency as IConnectionListener)?.Also(it => _connectListeners.Add(it.Interest, it));
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
	}
}