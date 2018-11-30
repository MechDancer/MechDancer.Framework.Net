using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using MechDancer.Framework.Net.Dependency;
using MechDancer.Framework.Net.Remote.Modules;
using MechDancer.Framework.Net.Remote.Modules.Multicast;
using MechDancer.Framework.Net.Remote.Modules.TcpConnection;
using MechDancer.Framework.Net.Remote.Protocol;
using MechDancer.Framework.Net.Remote.Resources;
using static MechDancer.Framework.Net.Dependency.Functions;

// ReSharper disable RedundantAssignment

namespace MechDancer.Framework.Net.Remote {
	public class RemoteHub {
		private readonly Group        _group = new Group();
		private readonly GroupMonitor _monitor;

		private readonly Networks             _networks    = new Networks();
		private readonly MulticastSockets     _sockets     = new MulticastSockets(Address);
		private readonly MulticastBroadcaster _broadcaster = new MulticastBroadcaster();
		private readonly MulticastReceiver    _receiver    = new MulticastReceiver();

		private readonly Addresses       _addresses     = new Addresses();
		private readonly ServerSockets   _servers       = new ServerSockets();
		private readonly PortBroadcaster _synchronizer1 = new PortBroadcaster();
		private readonly PortMonitor     _synchronizer2 = new PortMonitor();

		private readonly ShortConnectionClient _client = new ShortConnectionClient();
		private readonly ShortConnectionServer _server = new ShortConnectionServer();

		private readonly DynamicScope _scope;

		public RemoteHub(string                   name              = null,
		                 Action<string>           newMemberDetected = null,
		                 IEnumerable<IDependency> additional        = null
		) {
			_monitor = new GroupMonitor(newMemberDetected);
			_scope = Scope(@this => {
				               @this += new Name(name ?? RandomName);

				               @this += _group;
				               @this += _monitor;

				               @this += _networks;
				               @this += _sockets;
				               @this += _broadcaster;
				               @this += _receiver;

				               @this += _addresses;
				               @this += _servers;
				               @this += _synchronizer1;
				               @this += _synchronizer2;

				               @this += _client;
				               @this += _server;

				               if (additional != null)
					               foreach (var dependency in additional)
						               @this += dependency;
			               });
		}

		/// <summary>
		/// 	浏览全部依赖项
		/// </summary>
		public IReadOnlyCollection<IDependency> Modules => _scope.Dependencies;

		/// <summary>
		/// 	查看本机所有 可能打开的网络接口的IP地址 和 已经打开的服务套接字的端口号
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
		/// 	尝试打开一个随机的网络端口
		/// </summary>
		/// <remarks>
		///		若当前已有打开的网络端口则不进行任何操作
		/// </remarks>
		/// <returns>是否有网络端口已被打开</returns>
		public bool OpenOneNetwork() =>
			_sockets.View.Any()
		 || null != _networks.View
		                     .Keys
		                     .FirstOrDefault()
		                    ?.Also(it => _sockets.Get(it));

		/// <summary>
		/// 	打开本机所有网络端口对应的套接字
		/// </summary>
		/// <returns>打开的网络端口数量</returns>
		public int OpenAllNetworks() {
			foreach (var network in _networks.View.Keys)
				_sockets.Get(network);
			return _sockets.View.Count;
		}

		/// <summary>
		/// 	查看超时时间内出现的组成员
		/// </summary>
		/// <param name="timeout">超时时间</param>
		/// <returns>组成员列表</returns>
		public List<string> this[TimeSpan timeout] => _group[timeout];

		/// <summary>
		/// 	查看一个远端的地址和端口
		/// </summary>
		/// <param name="name">远端名字</param>
		public IPEndPoint this[string name] => _addresses[name];

		/// <summary>
		/// 	请求组成员自证存在性
		/// </summary>
		public void Yell() => _monitor.Yell();

		/// <summary>
		/// 	主动询问一个远端的端口号
		/// </summary>
		/// <param name="name">对方名字</param>
		public void Ask(string name) => _synchronizer2.Ask(name);

		/// <summary>
		/// 	广播数据
		/// </summary>
		/// <param name="cmd">指令代码</param>
		/// <param name="payload">数据负载</param>
		public void Broadcast(byte cmd, byte[] payload) => _broadcaster.Broadcast(cmd, payload);

		/// <summary>
		/// 	连接到一个TCP远端
		/// </summary>
		/// <param name="name">远端名字</param>
		/// <param name="cmd">建立连接的类型</param>
		/// <returns>
		/// 	打开的双向网络字节流
		///		尚未得知对方地址将直接返回空
		/// 	连接失败将返回空
		/// </returns>
		public NetworkStream Connect(string name, byte cmd) => _client.Connect(name, cmd);

		/// <summary>
		/// 	调度一次UDP组播接收
		/// </summary>
		/// <returns>
		/// 	收到的UDP包
		///		若收到的是自己发的则返回空
		/// </returns>
		public RemotePacket Invoke() => _receiver.Invoke();

		/// <summary>
		/// 	调度一次TCP短连接服务
		/// </summary>
		public void Accept() => _server.Invoke();

		private static readonly IPEndPoint Address
			= new IPEndPoint(IPAddress.Parse("233.33.33.33"), 23333);

		private static string RandomName => $"RemoteHub[{Guid.NewGuid()}]";
	}
}