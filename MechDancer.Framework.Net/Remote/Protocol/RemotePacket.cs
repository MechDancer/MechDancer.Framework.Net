using System;
using System.IO;

namespace MechDancer.Framework.Net.Remote.Protocol {
	public sealed class RemotePacket {
		public readonly byte   Command;
		public readonly string Sender;
		public readonly long   SeqNumber;
		public readonly byte[] Payload;

		public byte[] Bytes {
			get {
				var buffer = new MemoryStream(Payload.Length + 1);
				buffer.WriteByte(Command);
				buffer.WriteEnd(Sender);
				buffer.WriteZigzag(SeqNumber, false);
				buffer.Write(Payload);
				return new byte[0];
			}
		}

		public RemotePacket(
			byte   command,
			string sender,
			long   seqNumber,
			byte[] payload
		) {
			Command   = command;
			Sender    = sender;
			SeqNumber = seqNumber;
			Payload   = payload;
		}

		public RemotePacket(byte[] pack) {
			var buffer = new MemoryStream(pack);
			Command   = (byte) buffer.ReadByte();
			Sender    = buffer.ReadEnd();
			SeqNumber = buffer.ReadZigzag(false);
			Payload   = buffer.ReadRest();
		}
	}
}