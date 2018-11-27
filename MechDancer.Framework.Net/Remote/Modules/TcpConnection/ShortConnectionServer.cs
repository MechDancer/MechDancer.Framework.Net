using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MechDancer.Framework.Net.Dependency;
using MechDancer.Framework.Net.Remote.Resources;
using static MechDancer.Framework.Net.Dependency.Functions;

namespace MechDancer.Framework.Net.Remote.Modules.TcpConnection {
	public sealed class ShortConnectionServer : AbstractModule {
		private readonly Lazy<ServerSockets> _servers;

		private readonly HashSet<IShortConnectionListener> _listeners
			= new HashSet<IShortConnectionListener>();

		public ShortConnectionServer() => _servers = Must<ServerSockets>(Host);

		public override void Sync() {
			lock (_listeners) {
				_listeners.Clear();
				foreach (var listener
					in Host.Value.Get<IShortConnectionListener>()
				) _listeners.Add(listener);
			}

			new List<byte>()
			   .Also(list => {
				         foreach (var listener in _listeners)
					         list.AddRange(listener.Interest);
			         })
			   .Let(list => list.Distinct().ToList().Count != list.Count)
			   .Also(reduplicate => {
				         if (reduplicate) throw new AmbiguousMatchException(ReduplicateErrorMsg);
			         });
		}

		public void Invoke(int port = 0) {
			using (var stream = _servers.Value.Get(port).AcceptTcpClient().GetStream())
				stream.Listen(it => (TcpCmd) it)
				      .Let(cmd => _listeners.SingleOrDefault(it => it.Interest.Contains((byte) cmd)))
				     ?.Process(stream);
		}

		public override bool Equals(object obj) => obj is ShortConnectionServer;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(ShortConnectionServer).GetHashCode();

		private const string ReduplicateErrorMsg = "more than one listener interested in same command";
	}
}