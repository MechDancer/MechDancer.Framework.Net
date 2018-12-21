using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Net.Modules;
using MechDancer.Framework.Net.Modules.Multicast;
using MechDancer.Framework.Net.Modules.TcpConnection;
using MechDancer.Framework.Net.Protocol;
using MechDancer.Framework.Net.Resources;

// ReSharper disable RedundantAssignment

namespace MechDancer.Framework.Net {
	public class RemoteHub {
		private static readonly IPEndPoint Address
			= new IPEndPoint(IPAddress.Parse("233.33.33.33"), 23333);

		private readonly Addresses            _addresses = new Addresses();
		private readonly MulticastBroadcaster _broadcaster;

		private readonly ShortConnectionClient _client = new ShortConnectionClient();

		private readonly Group        _group = new Group();
		private readonly GroupMonitor _monitor;

		private readonly Networks          _networks = new Networks();
		private readonly MulticastReceiver _receiver = new MulticastReceiver();

		private readonly DynamicScope          _scope;
		private readonly ShortConnectionServer _server  = new ShortConnectionServer();
		private readonly ServerSockets         _servers = new ServerSockets();
		private readonly PacketSlicer          _slicer  = new PacketSlicer();
		private readonly MulticastSockets      _sockets;
		private readonly PortBroadcaster       _synchronizer1 = new PortBroadcaster();
		private readonly PortMonitor           _synchronizer2 = new PortMonitor();

		public RemoteHub(string                  name              = null,
		                 uint                    size              = 0x4000,
		                 IPEndPoint              address           = null,
		                 Action<string>          newMemberDetected = null,
		                 IEnumerable<IComponent> additions         = null
		) {
			_monitor     = new GroupMonitor(detected: newMemberDetected);
			_sockets     = new MulticastSockets(address ?? Address);
			_broadcaster = new MulticastBroadcaster(size);

			_scope = new DynamicScope();
			_scope.Setup(new Name(name ?? RandomName));

			_scope.Setup(_group);
			_scope.Setup(_monitor);

			_scope.Setup(_networks);
			_scope.Setup(_sockets);
			_scope.Setup(_broadcaster);
			_scope.Setup(_receiver);
			_scope.Setup(_slicer);

			_scope.Setup(_addresses);
			_scope.Setup(_servers);
			_scope.Setup(_synchronizer1);
			_scope.Setup(_synchronizer2);

			_scope.Setup(_client);
			_scope.Setup(_server);

			if (additions != null)
				foreach (var dependency in additions)
					_scope.Setup(dependency);
		}

		/// <summary>
		///     浏览全部依赖项
		/// </summary>
		public IEnumerable<IComponent> Modules => _scope.Components;

		/// <summary>
		///     查看本机所有 可能打开的网络接口的IP地址 和 已经打开的服务套接字的端口号
		/// </summary>
		public (ICollection<IPAddress>, ICollection<int>) Endpoints
			=> (_networks.View
			             .Values
			             .Select(it => it.Address)
			             .ToList(),
			    _servers.View
			            .Keys
			            .ToList()
			            .Also(list => _servers.Default
			                                  .LocalEndpoint
			                                  .Let(it => (IPEndPoint) it)
			                                  .Port
			                                  .Also(list.Add))
			   );

		/// <summary>
		///     查看超时时间内出现的组成员
		/// </summary>
		/// <param name="timeout">超时时间</param>
		/// <returns>组成员列表</returns>
		public List<string> this[TimeSpan timeout] => _group[timeout];

		/// <summary>
		///     查看一个远端的地址和端口
		/// </summary>
		/// <param name="name">远端名字</param>
		public IPEndPoint this[string name] => _addresses[name];

		private static string RandomName => $"RemoteHub[{Guid.NewGuid()}]";

		/// <summary>
		///     尝试打开一个随机的网络端口
		/// </summary>
		/// <remarks>
		///     若当前已有打开的网络端口则不进行任何操作
		/// </remarks>
		/// <returns>是否有网络端口已被打开</returns>
		public bool OpenOneNetwork()
			=> _sockets.View.Any()
			|| null != _networks.View
			                    .Values
			                    .FirstOrDefault()
			                   ?.Address
			                    .Also(it => _sockets.Get(it));

		/// <summary>
		///     打开本机所有网络端口对应的套接字
		/// </summary>
		/// <returns>打开的网络端口数量</returns>
		public int OpenAllNetworks() {
			foreach (var address in _networks.View.Values)
				_sockets.Get(address.Address);
			return _sockets.View.Count;
		}

		/// <summary>
		///     请求组成员自证存在性
		/// </summary>
		public void Yell() => _monitor.Yell();

		/// <summary>
		///     主动询问一个远端的端口号
		/// </summary>
		/// <param name="name">对方名字</param>
		public void Ask(string name) => _synchronizer2.Ask(name);

		/// <summary>
		///     广播数据
		/// </summary>
		/// <param name="cmd">指令代码</param>
		/// <param name="payload">数据负载</param>
		public void Broadcast(byte cmd, byte[] payload)
			=> _broadcaster.Broadcast(cmd, payload);

		/// <summary>
		///     连接到一个TCP远端
		/// </summary>
		/// <param name="name">远端名字</param>
		/// <param name="cmd">建立连接的类型</param>
		/// <param name="block">建立连接成功后执行方法</param>
		/// <returns>
		///     是否执行了方法
		///     尚未得知对方地址或连接失败将返回 false
		/// </returns>
		public bool Connect(string name, byte cmd, Action<NetworkStream> block) {
			using (var client = _client.Connect(name, cmd)) {
				return client?.Also(block) != null;
			}
		}

		/// <summary>
		///     调度一次UDP组播接收
		/// </summary>
		/// <returns>
		///     收到的UDP包
		///     若收到的是自己发的则返回空
		/// </returns>
		public RemotePacket Invoke() => _receiver.Invoke();

		/// <summary>
		///     调度一次TCP短连接服务
		/// </summary>
		public void Accept() => _server.Invoke();
	}
}