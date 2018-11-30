using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using MechDancer.Framework.Net.Dependency;
using MechDancer.Framework.Net.Remote.Protocol;
using MechDancer.Framework.Net.Remote.Resources;
using static MechDancer.Framework.Net.Dependency.Functions;

namespace MechDancer.Framework.Net.Remote.Modules.Multicast {
	/// <summary>
	/// 	组播单体接收
	/// </summary>
	public sealed class MulticastReceiver : AbstractModule {
		private readonly ThreadLocal<byte[]>         _buffer;    // 线程独立缓冲区
		private readonly Lazy<Name>                  _name;      // 过滤环路数据
		private readonly Lazy<MulticastSockets>      _socket;    // 接收套接字
		private readonly HashSet<IMulticastListener> _listeners; // 处理回调

		private readonly Lazy<Networks>  _networks;  // 网络管理
		private readonly Lazy<Addresses> _addresses; // 地址管理

		/// <summary>
		/// 	构造器
		/// </summary>
		/// <param name="bufferSize">缓冲区容量</param>
		public MulticastReceiver(uint bufferSize = 65536) {
			_buffer    = new ThreadLocal<byte[]>(() => new byte[bufferSize]);
			_name      = Maybe<Name>();
			_socket    = Must<MulticastSockets>();
			_networks  = Maybe<Networks>();
			_addresses = Maybe<Addresses>();
			_listeners = new HashSet<IMulticastListener>();
		}

		public override void Sync() {
			foreach (var listener in Dependencies.Get<IMulticastListener>())
				_listeners.Add(listener);
		}

		public RemotePacket Invoke() {
			var length = _socket.Value.Default.ReceiveFrom(_buffer.Value, out var address);

			var stream = new MemoryStream(_buffer.Value, 0, length, false);
			var sender = stream.ReadEnd();

			if (sender == (_name.Value?.Field ?? "")) return null;

			_networks.Value
			        ?.View
			         .FirstOrDefault(it => Match(it.Value, address))
			         .Key
			        ?.Let(it => _socket.Value.Get(it));
			_addresses.Value?.Update(sender, address);

			var packet = new RemotePacket
				(sender: sender,
				 command: (byte) stream.ReadByte(),
				 payload: stream.ReadRest());

			// Console.WriteLine($"{packet} from {address}");

			foreach (var listener in from item in _listeners
			                         where item.Interest.Contains(packet.Command)
			                         select item)
				listener.Process(packet);

			return packet;
		}

		public override bool Equals(object obj) => obj is MulticastReceiver;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(MulticastReceiver).GetHashCode();

		/// <summary>
		/// 	判定两个网络在同一个子网中
		/// </summary>
		/// <param name="this">包含子网掩码的本机网络地址</param>
		/// <param name="other">一个外部的网络地址</param>
		/// <returns>是否同属一个子网</returns>
		private static bool Match(UnicastIPAddressInformation @this, IPAddress other) {
			if (Equals(@this.Address, other)) return true;
			if (@this.Address.AddressFamily != AddressFamily.InterNetwork) return false;

			var mask = @this.IPv4Mask.GetAddressBytes();
			var a    = @this.Address.GetAddressBytes();
			var b    = other.GetAddressBytes();
			for (var i = 0; i < 4; ++i)
				if ((a[i] & mask[i]) != (b[i] & mask[i]))
					return false;
			return true;
		}
	}
}