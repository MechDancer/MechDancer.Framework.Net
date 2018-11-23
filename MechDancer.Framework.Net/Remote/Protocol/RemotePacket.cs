using System;

namespace MechDancer.Framework.Net.Remote.Protocol {
	public sealed class RemotePacket {
		public readonly  byte         Command;
		public readonly  string       Sender;
		public readonly  long         SeqNumber;
		public readonly  byte[]       Payload;
		private readonly Lazy<byte[]> _bytes;

		public byte[] Bytes => _bytes.Value;

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
			_bytes    = new Lazy<byte[]>(() => { return new byte[0]; });
		}

		public RemotePacket(byte[] pack) {
			// Command   = command;
			// Sender    = sender;
			// SeqNumber = seqNumber;
			// Payload   = payload;
			_bytes = new Lazy<byte[]>(() => pack);
		}
	}
}