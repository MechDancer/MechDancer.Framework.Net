using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Net.Modules;
using MechDancer.Framework.Net.Modules.Multicast;
using MechDancer.Framework.Net.Modules.TcpConnection;
using MechDancer.Framework.Net.Resources;

namespace MechDancer.Framework.Net.Presets {
	/// <summary>
	///     探针
	/// </summary>
	public sealed class Probe {
		private readonly Addresses _addresses = new Addresses();

		// 重要资源
		private readonly Group _group = new Group();

		// 组播
		private readonly MulticastMonitor  _monitor  = new MulticastMonitor();
		private readonly MulticastReceiver _receiver = new MulticastReceiver();
		private readonly MulticastSockets  _sockets;

		/// <summary>
		///     构造器
		/// </summary>
		/// <param name="group">组播地址和端口</param>
		public Probe(IPEndPoint group = null) {
			var scope = new DynamicScope();

			scope.Setup(_group);
			scope.Setup(_addresses);
			scope.Setup(new GroupMonitor());
			scope.Setup(new PortMonitor());

			scope.Setup(new Networks());
			scope.Setup(_sockets = new MulticastSockets(group ?? Default.Group));
			scope.Setup(_monitor);
			scope.Setup(_receiver);

			_monitor.BindAll();
		}

		/// <summary>
		///     查看探针所在的组播组
		/// </summary>
		public IPEndPoint Group => _sockets.Group;

		/// <summary>
		///     查看所有成员的信息
		/// </summary>
		public IReadOnlyDictionary<string, (DateTime? time, IPEndPoint address)> View
			=> _group.View
					 .Keys
					 .ToDictionary(name => name, name => this[name]);

		/// <summary>
		///     查看所有在线的成员
		/// </summary>
		/// <param name="timeout">超时时间</param>
		public IEnumerable<string> this[TimeSpan timeout]
			=> _group[timeout];

		/// <summary>
		///     查看一个成员的信息
		/// </summary>
		/// <param name="name">名字</param>
		public (DateTime? time, IPEndPoint address) this[string name]
			=> (_group[name], _addresses[name]);

		/// <summary>
		///     重新扫描并绑定所有本地网络接口
		/// </summary>
		public void Scan()
			=> _monitor.BindAll(true);

		/// <summary>
		///     调用一次阻塞接收
		/// </summary>
		public void Invoke() => _receiver.Invoke();
	}
}