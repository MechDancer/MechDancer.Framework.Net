using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using MechDancer.Framework.Dependency;

namespace MechDancer.Framework.Net.Remote.Resources {
	public sealed class ServerSockets : IComponent {
		private readonly ConcurrentDictionary<int, TcpListener> _core
			= new ConcurrentDictionary<int, TcpListener>();

		private readonly Lazy<TcpListener> _default;

		public IReadOnlyDictionary<int, TcpListener> View    => _core;
		public TcpListener                           Default => _default.Value;

		public ServerSockets(int port = 0) {
			_default = new Lazy<TcpListener>(() => Server(port));
		}

		public TcpListener Get(int parameter) =>
			parameter == 0 ? Default : _core.GetOrAdd(parameter, Server);

		public override bool Equals(object obj) => obj is ServerSockets;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(ServerSockets).GetHashCode();

		private static TcpListener Server(int port)
			=> new TcpListener(IPAddress.Any, port).Also(it => it.Start());
	}
}