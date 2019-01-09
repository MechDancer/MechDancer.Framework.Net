using System;
using System.IO;
using MechDancer.Common;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Dependency.UniqueComponent;
using MechDancer.Framework.Net.Resources;

namespace MechDancer.Framework.Net.Modules.Multicast {
	/// <inheritdoc cref="UniqueComponent{T}" />
	/// <inheritdoc cref="IDependent" />
	/// <summary>
	///     组播发布者
	/// </summary>
	public sealed class MulticastBroadcaster : UniqueComponent<MulticastBroadcaster>, IDependent {
		private readonly UniqueDependencies _dependencies = new UniqueDependencies();

		private readonly UniqueDependency<Name> _name;

		private readonly int                                _size;
		private readonly UniqueDependency<PacketSlicer>     _slicer;
		private readonly UniqueDependency<MulticastSockets> _sockets;

		public MulticastBroadcaster(uint size = 0x4000) {
			_size    = size <= 65536 ? (int) size : throw new ArgumentException($"size {size} > 65536");
			_name    = _dependencies.BuildDependency<Name>();
			_slicer  = _dependencies.BuildDependency<PacketSlicer>();
			_sockets = _dependencies.BuildDependency<MulticastSockets>();
		}

		public bool Sync(IComponent dependency) => _dependencies.Sync(dependency);

		public void Broadcast(byte cmd, byte[] payload = null) {
			payload = payload ?? new byte[0];
			var me = _name.Field?.Field;

			if (string.IsNullOrWhiteSpace(me)
			 && (cmd == (byte) UdpCmd.YellAck || cmd == (byte) UdpCmd.AddressAck)
			) return;

			var stream = new MemoryStream(_size);
			stream.WriteEnd(me ?? "");

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
			} else {
				throw new OutOfMemoryException("payload is too heavy");
			}
		}
	}
}