using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using MechDancer.Framework.Dependency;

namespace MechDancer.Framework.Net.Resources {
	public sealed class ServerSockets : AbstractComponent<ServerSockets> {
		private readonly ConcurrentDictionary<int, TcpListener> _core
			= new ConcurrentDictionary<int, TcpListener>();

		private readonly Lazy<TcpListener> _default;

		public ServerSockets(int port = 0)
			=> _default = new Lazy<TcpListener>(() => Server(port));

		public IReadOnlyDictionary<int, TcpListener> View    => _core;
		public TcpListener                           Default => _default.Value;

		public TcpListener Get(int parameter)
			=> parameter == 0 ? Default : _core.GetOrAdd(parameter, Server);

		private static TcpListener Server(int port)
			=> new TcpListener(IPAddress.Any, port).Also(it => it.Start());
	}
}