using System.Collections.Concurrent;
using System.Net;
using MechDancer.Framework.Dependency;

namespace MechDancer.Framework.Net.Remote.Resources {
	public sealed class Addresses : IComponent {
		private readonly ConcurrentDictionary<string, IPEndPoint> _core
			= new ConcurrentDictionary<string, IPEndPoint>();

		public IPEndPoint Update(string name, IPAddress address) =>
			_core.AddOrUpdate
				(name,
				 _ => new IPEndPoint(address,         0),
				 (_, last) => new IPEndPoint(address, last.Port));

		public IPEndPoint Update(string name, int port) =>
			_core.AddOrUpdate
				(name,
				 _ => new IPEndPoint(IPAddress.Any,        port),
				 (_, last) => new IPEndPoint(last.Address, port));

		public IPEndPoint Update(string name, IPEndPoint address) =>
			_core[name] = address;

		public IPEndPoint this[string name] =>
			_core.TryGetValue(name, out var endPoint) && endPoint.Port != 0
				? endPoint
				: null;

		public bool Remove(string name) =>
			_core.TryRemove(name, out _);

		public override bool Equals(object obj) => obj is Addresses;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(Addresses).GetHashCode();
	}
}