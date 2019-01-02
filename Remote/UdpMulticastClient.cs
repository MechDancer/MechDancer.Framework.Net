using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using MechDancer.Common;

namespace MechDancer.Framework.Net {
	/// <inheritdoc />
	/// <summary>
	///     UDP 组播客户端
	/// </summary>
	public sealed class UdpMulticastClient : IDisposable {
		private readonly IPEndPoint _multicast;

		/// <summary>
		///     底层套接字
		/// </summary>
		public readonly Socket Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

		/// <summary>
		///     构造 UDP 组播客户端
		/// </summary>
		/// <param name="multicast">组播地址和端口</param>
		/// <param name="interface">目标出口网卡地址</param>
		public UdpMulticastClient(IPEndPoint multicast, NetworkInterface @interface) {
			_multicast = multicast;
			// 允许端口复用
			Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, false);
			Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress,        true);
			// 绑定端口
			Socket.Bind(new IPEndPoint(IPAddress.Any, multicast.Port));
			// 获取网卡序号并指定出口网卡
			@interface
			  ?.GetIPProperties()
			   .GetIPv4Properties()
			   .Index
			   .Also(index => Socket.SetSocketOption
				         (SocketOptionLevel.IP,
				          SocketOptionName.MulticastInterface,
				          IPAddress.HostToNetworkOrder(index)));
		}

		/// <inheritdoc />
		/// <summary>
		///     释放网络资源
		/// </summary>
		public void Dispose() => Socket.Dispose();

		/// <summary>
		///     监听指定网络接口上到来的组播包
		/// </summary>
		/// <param name="local">网络端口的单播地址</param>
		public void Bind(IPAddress local) {
			try {
				Socket.SetSocketOption
					(SocketOptionLevel.IP,
					 SocketOptionName.AddMembership,
					 local == null
						 ? new MulticastOption(_multicast.Address)
						 : new MulticastOption(_multicast.Address, local));
			} catch (SystemException) { }
		}

		/// <summary>
		///     向组播发送数据
		/// </summary>
		/// <param name="payload">负载数据包</param>
		/// <param name="size">数据包范围</param>
		public void Broadcast(byte[] payload, int size) => Socket.SendTo(payload, size, SocketFlags.None, _multicast);

		/// <summary>
		///     从组播接收数据
		/// </summary>
		/// <param name="buffer">存放数据包的缓存</param>
		/// <param name="address">数据包到来的地址</param>
		/// <returns>收到的包长度</returns>
		public int ReceiveFrom(byte[] buffer, out IPAddress address) {
			EndPoint temp   = new IPEndPoint(IPAddress.Any, 0);
			var      result = Socket.ReceiveFrom(buffer, ref temp);
			address = ((IPEndPoint) temp).Address;
			return result;
		}

		/// <summary>
		///     通过IP获取网卡
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