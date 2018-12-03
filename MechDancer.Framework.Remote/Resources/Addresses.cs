using System.Collections.Concurrent;
using System.Net;
using MechDancer.Framework.Dependency;

namespace MechDancer.Framework.Net.Resources {
	public sealed class Addresses : IComponent {
		private static readonly int Hash = typeof(Addresses).GetHashCode();

		private readonly ConcurrentDictionary<string, IPEndPoint> _core
			= new ConcurrentDictionary<string, IPEndPoint>();

		public IPEndPoint this[string name] =>
			_core.TryGetValue(name, out var endPoint) && endPoint.Port != 0
				? endPoint
				: null;

		public IPEndPoint Update(string name, IPAddress address) {
			return _core.AddOrUpdate
				(name,
				 _ => new IPEndPoint(address,         0),
				 (_, last) => new IPEndPoint(address, last.Port));
		}

		public IPEndPoint Update(string name, int port) {
			return _core.AddOrUpdate
				(name,
				 _ => new IPEndPoint(IPAddress.Any,        port),
				 (_, last) => new IPEndPoint(last.Address, port));
		}

		public IPEndPoint Update(string name, IPEndPoint address) {
			return _core[name] = address;
		}

		public bool Remove(string name) {
			return _core.TryRemove(name, out _);
		}

		public override bool Equals(object obj) {
			return obj is Addresses;
		}

		public override int GetHashCode() {
			return Hash;
		}
	}
}