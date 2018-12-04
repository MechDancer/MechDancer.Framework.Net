using System;
using System.IO;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Net.Protocol;
using MechDancer.Framework.Net.Resources;

namespace MechDancer.Framework.Net.Modules.Multicast {
	/// <summary>
	///     组播发布者
	/// </summary>
	public sealed class MulticastBroadcaster : AbstractDependent<MulticastBroadcaster> {
		private readonly int _size;

		private readonly ComponentHook<Name>             _name; // 可以匿名发送组播
		private readonly ComponentHook<PacketSlicer>     _slicer;
		private readonly ComponentHook<MulticastSockets> _sockets;

		public MulticastBroadcaster(uint size = 0x4000) {
			_size    = (int) size;
			_name    = BuildDependency<Name>();
			_slicer  = BuildDependency<PacketSlicer>();
			_sockets = BuildDependency<MulticastSockets>();
		}

		public void Broadcast(byte cmd, byte[] payload = null) {
			payload = payload ?? new byte[0];
			var me = _name.Field?.Field;

			if (string.IsNullOrWhiteSpace(me)
			 && (cmd == (byte) UdpCmd.YellAck || cmd == (byte) UdpCmd.AddressAck)
			) return;

			var stream = new MemoryStream(_size);
			stream.WriteEnd(me);

			void Send() {
				foreach (var socket in _sockets.StrictField.View.Values)
					socket.Broadcast(stream.GetBuffer(), (int) stream.Position);
			}

			if (stream.Available() - 1 >= payload.Length) {
				stream.WriteByte(cmd);
				stream.Write(payload);
				Send();
			} else if (_slicer.Field != null) {
				stream.WriteByte((byte) UdpCmd.PackageSlice);
				var position = stream.Position;
				_slicer.StrictField.Broadcast
					(cmd, payload,
					 (int) stream.Available(),
					 bytes => {
						 stream.Position = position;
						 stream.Write(bytes);
						 Send();
					 });
			} else throw new OutOfMemoryException("payload is too heavy");
		}
	}
}