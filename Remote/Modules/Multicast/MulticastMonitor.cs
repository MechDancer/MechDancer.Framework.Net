using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using MechDancer.Common;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Dependency.UniqueComponent;
using MechDancer.Framework.Net.Resources;

namespace MechDancer.Framework.Net.Modules.Multicast {
	using Predicate = Func<KeyValuePair<NetworkInterface, UnicastIPAddressInformation>, bool>;

	/// <summary>
	///     组播管理
	/// </summary>
	public sealed class MulticastMonitor : UniqueComponent<MulticastMonitor>, IDependent {
		private readonly UniqueDependencies         _dependencies = new UniqueDependencies();
		private readonly UniqueDependency<Networks> _networks;

		private readonly UniqueDependency<MulticastSockets> _sockets;

		public MulticastMonitor() {
			_sockets  = _dependencies.BuildDependency<MulticastSockets>();
			_networks = _dependencies.BuildDependency<Networks>();
		}

		public bool Sync(IComponent dependency) => _dependencies.Sync(dependency);

		/// <summary>
		///     绑定特定地址
		/// </summary>
		/// <param name="address">地址</param>
		public void Bind(IPAddress address) => _sockets.StrictField.Bind(address);

		/// <summary>
		///     打开特定接口
		/// </summary>
		/// <param name="interface">网络接口</param>
		public void Open(NetworkInterface @interface) => _sockets.StrictField.Open(@interface);

		/// <summary>
		///     绑定一个网络接口
		/// </summary>
		/// <param name="scan">绑定前扫描</param>
		public void BindOne(bool scan = false) {
			if (scan) _networks.StrictField.Scan();
			_networks.StrictField
			         .View
			         .Values
			         .FirstOrDefault()
			        ?.Address
			         .Also(_sockets.StrictField.Bind);
		}

		/// <summary>
		///     绑定所有符合条件的网络接口
		/// </summary>
		/// <param name="predicate">判断条件</param>
		/// <param name="scan">绑定前扫描</param>
		public void BindWhere(Predicate predicate, bool scan = false) {
			if (scan) _networks.StrictField.Scan();
			foreach (var address in _networks.StrictField
			                                 .View
			                                 .Where(predicate)
			                                 .Select(it => it.Value.Address)
			) _sockets.StrictField.Bind(address);
		}

		/// <summary>
		///     绑定所有网络接口
		/// </summary>
		/// <param name="scan">绑定前扫描</param>
		public void BindAll(bool scan = false) {
			if (scan) _networks.StrictField.Scan();
			foreach (var info in _networks.StrictField.View.Values)
				_sockets.StrictField.Bind(info.Address);
		}

		/// <summary>
		///     打开一个网络接口
		/// </summary>
		/// <param name="scan">打开前扫描</param>
		/// <returns>打开的网络接口</returns>
		public IEnumerable<NetworkInterface> OpenOne(bool scan = false) {
			if (scan) _networks.StrictField.Scan();
			_networks.StrictField
			         .View
			         .Keys
			         .FirstOrDefault()
			        ?.Also(_sockets.StrictField.Open);
			return _sockets.StrictField.View.Keys;
		}

		/// <summary>
		///     打开所有符合条件的网络接口
		/// </summary>
		/// <param name="predicate">判断条件</param>
		/// <param name="scan">打开前扫描</param>
		/// <returns>打开的网络接口</returns>
		public IEnumerable<NetworkInterface> OpenWhere(Predicate predicate, bool scan = false) {
			if (scan) _networks.StrictField.Scan();
			foreach (var @interface in _networks.StrictField
			                                    .View
			                                    .Where(predicate)
			                                    .Select(it => it.Key)
			) _sockets.StrictField.Open(@interface);
			return _sockets.StrictField.View.Keys;
		}

		/// <summary>
		///     打开所有网络接口
		/// </summary>
		/// <param name="scan">打开前扫描</param>
		/// <returns>打开的网络接口</returns>
		public IEnumerable<NetworkInterface> OpenAll(bool scan = false) {
			if (scan) _networks.StrictField.Scan();
			foreach (var @interface in _networks.StrictField.View.Keys)
				_sockets.StrictField.Open(@interface);
			return _sockets.StrictField.View.Keys;
		}
	}
}