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
	public sealed class RemoteHub {
		private readonly Group        _group;
		private readonly GroupMonitor _monitor;

		private readonly Networks          _networks;
		private readonly MulticastSockets  _sockets;
		private readonly MulticastReceiver _receiver;
		private readonly CommonMulticast   _commonMulticast;

		private readonly Addresses       _addresses;
		private readonly ServerSockets   _servers;
		private readonly PortBroadcaster _synchronizer1;
		private readonly PortMonitor     _synchronizer2;

		private readonly ShortConnectionClient _client;
		private readonly ShortConnectionServer _server;

		public readonly DynamicScope Hub;

		public RemoteHub(string name) {
			_group           = new Group();
			_monitor         = new GroupMonitor();
			_networks        = new Networks();
			_sockets         = new MulticastSockets(Address);
			_receiver        = new MulticastReceiver();
			_commonMulticast = new CommonMulticast(null);
			_addresses       = new Addresses();
			_servers         = new ServerSockets();
			_synchronizer1   = new PortBroadcaster();
			_synchronizer2   = new PortMonitor();
			_client          = new ShortConnectionClient();
			_server          = new ShortConnectionServer();

			Hub = Scope(@this => {
				            @this += new Name(name);

				            @this += _group;
				            @this += _monitor;

				            @this += _networks;
				            @this += _sockets;
				            @this += new MulticastBroadcaster();
				            @this += _receiver;
				            @this += _commonMulticast;

				            @this += _addresses;
				            @this += _servers;
				            @this += _synchronizer1;
				            @this += _synchronizer2;

				            @this += _client;
				            @this += _server;
			            });
		}

		/// <summary>
		/// 	打开本机所有网络端口对应的套接字
		/// </summary>
		public void OpenAllNetworks() {
			foreach (var network in _networks.View.Keys)
				_sockets.Get(network);
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
		/// 	发送通用广播
		/// </summary>
		/// <param name="payload">广播数据包</param>
		public void Broadcast(byte[] payload) => _commonMulticast.Broadcast(payload);

		/// <summary>
		/// 	主动询问一个远端的端口号
		/// </summary>
		/// <param name="name">对方名字</param>
		public void Ask(string name) => _synchronizer2.Ask(name);

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
	}
}