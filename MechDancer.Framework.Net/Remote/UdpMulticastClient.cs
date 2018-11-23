using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace MechDancer.Framework.Net.Remote {
	/// <inheritdoc />
	/// <summary>
	///     UDP 组播客户端
	/// </summary>
	public class UdpMulticastClient : IDisposable {
		private readonly IPEndPoint _multicast;

		/// <summary>
		/// 	底层套接字
		/// </summary>
		public readonly Socket Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

		/// <summary>
		///     构造 UDP 组播客户端
		/// </summary>
		/// <param name="multicast">组播地址和端口</param>
		/// <param name="networkInterface">目标出口网卡</param>
		public UdpMulticastClient
		(IPEndPoint       multicast,
		 NetworkInterface networkInterface
		) {
			_multicast = multicast;
			// 允许端口复用
			Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			// 绑定端口
			Socket.Bind(new IPEndPoint(IPAddress.Any, multicast.Port));
			// 加入组播
			Socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
			                       new MulticastOption(multicast.Address));
			if (networkInterface == null) return;
			// 获取网卡序号
			var index = networkInterface.GetIPProperties().GetIPv4Properties().Index;
			// 指定出口网卡
			Socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface,
			                       IPAddress.HostToNetworkOrder(index));
		}

		/// <inheritdoc />
		/// <summary>
		///     释放网络资源
		/// </summary>
		public void Dispose() => Socket.Dispose();

		/// <summary>
		///     向组播发送数据
		/// </summary>
		/// <param name="payload">负载数据包</param>
		public void Broadcast(byte[] payload) => Socket.SendTo(payload, _multicast);

		/// <summary>
		///     从组播接收数据
		/// </summary>
		/// <param name="buffer">存放数据包的缓存</param>
		/// <returns>收到的包长度</returns>
		public ushort Receive(byte[] buffer) => (ushort) Socket.Receive(buffer);

		/// <summary>
		/// 	通过IP获取网卡
		/// </summary>
		/// <param name="localAddress">网卡绑定的单播IP</param>
		/// <returns>网卡引用</returns>
		public static NetworkInterface GetAdapterByAddress(IPAddress localAddress) =>
			(from adapter in NetworkInterface.GetAllNetworkInterfaces()
			 where adapter.GetIPProperties()
			              .UnicastAddresses
			              .Select(x => x.Address)
			              .Contains(localAddress)
			 select adapter).FirstOrDefault();
	}
}