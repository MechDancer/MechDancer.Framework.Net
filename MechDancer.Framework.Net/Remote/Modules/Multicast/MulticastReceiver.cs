using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		private readonly MaybeProperty<Name>         _name;      // 过滤环路数据
		private readonly Lazy<MulticastSockets>      _socket;    // 接收套接字
		private readonly HashSet<IMulticastListener> _callbacks; // 处理回调

		private readonly MaybeProperty<Networks>  _networks;  // 网络管理
		private readonly MaybeProperty<Addresses> _addresses; // 地址管理

		/// <summary>
		/// 	构造器
		/// </summary>
		/// <param name="bufferSize">缓冲区容量</param>
		public MulticastReceiver(uint bufferSize = 65536) {
			_buffer    = new ThreadLocal<byte[]>(() => new byte[bufferSize]);
			_name      = Maybe<Name>(Host);
			_socket    = Must<MulticastSockets>(Host);
			_networks  = Maybe<Networks>(Host);
			_addresses = Maybe<Addresses>(Host);
			_callbacks = new HashSet<IMulticastListener>();
		}

		public override void Sync() {
			foreach (var listener in Host.Value.Get<IMulticastListener>())
				_callbacks.Add(listener);
		}

		public RemotePacket Invoke() {
			var length = _socket.Value.Default.ReceiveFrom(_buffer.Value, out var address);

			if (_networks.Get(out var networks))
				networks.View
				        .ToList()
				        .Select(it => Tuple.Create(it.Key, it.Value))
				        .FirstOrDefault(it => Equals(it.Item2, address))
				       ?.Item1
				       ?.Let(network => _socket.Value.Get(network));

			var stream = new MemoryStream(_buffer.Value, 0, length, false);
			var sender = stream.ReadEnd();

			if (sender == (_name.Get(out var name) ? name.Field : "")) return null;

			if (_addresses.Get(out var addresses))
				addresses.Update(sender, address);

			var packet = new RemotePacket
				(sender: sender,
				 command: (byte) stream.ReadByte(),
				 payload: stream.ReadRest());

			// Console.WriteLine($"{packet} from {address}");

			foreach (var listener in
				_callbacks.Where(it => it.Interest
				                         .Contains(packet.Command))
			) listener.Process(packet);

			return packet;
		}

		public override bool Equals(object obj) => obj is MulticastReceiver;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(MulticastReceiver).GetHashCode();
	}
}