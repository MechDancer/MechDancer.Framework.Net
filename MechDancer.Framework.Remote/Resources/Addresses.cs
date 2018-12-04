using System.Collections.Concurrent;
using System.Net;
using MechDancer.Framework.Dependency;

namespace MechDancer.Framework.Net.Resources {
	public sealed class Addresses : AbstractComponent<Addresses> {
		private readonly ConcurrentDictionary<string, IPEndPoint> _core
			= new ConcurrentDictionary<string, IPEndPoint>();

		public IPEndPoint this[string name] =>
			_core.TryGetValue(name, out var endPoint) && endPoint.Port != 0
				? endPoint
				: null;

		public IPEndPoint Update(string name, IPAddress address)
			=> _core.AddOrUpdate
				(name,
				 _ => new IPEndPoint(address,         0),
				 (_, last) => new IPEndPoint(address, last.Port));

		public IPEndPoint Update(string name, int port)
			=> _core.AddOrUpdate
				(name,
				 _ => new IPEndPoint(IPAddress.Any,        port),
				 (_, last) => new IPEndPoint(last.Address, port));

		public IPEndPoint Update(string name, IPEndPoint address) => _core[name] = address;

		public bool Remove(string name) => _core.TryRemove(name, out _);
	}
}