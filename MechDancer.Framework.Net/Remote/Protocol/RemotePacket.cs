using System;
using System.IO;

namespace MechDancer.Framework.Net.Remote.Protocol {
	public sealed class RemotePacket {
		public readonly string Sender;
		public readonly byte   Command;
		public readonly long   SeqNumber;
		public readonly byte[] Payload;

		/// <summary>
		/// 	从结构化数据构造
		/// </summary>
		public RemotePacket(
			string sender,
			byte   command,
			long   seqNumber,
			byte[] payload
		) {
			Command   = command;
			Sender    = sender;
			SeqNumber = seqNumber;
			Payload   = payload;
		}

		/// <summary>
		/// 	从数据包构造
		/// </summary>
		public RemotePacket(Stream buffer) {
			Sender    = buffer.ReadEnd();
			Command   = (byte) buffer.ReadByte();
			SeqNumber = buffer.ReadZigzag(false);
			Payload   = buffer.ReadRest();
		}

		/// <summary>
		/// 	解构
		/// </summary>
		public void Deconstruct(
			out string sender,
			out byte   command,
			out long   seqNumber,
			out byte[] payload
		) {
			sender    = Sender;
			command   = Command;
			seqNumber = SeqNumber;
			payload   = Payload;
		}

		public byte[] Bytes =>
			new MemoryStream(Payload.Length + 1)
			   .Also(it => {
						 it.WriteByte(Command);
						 it.WriteEnd(Sender);
						 it.WriteZigzag(SeqNumber, false);
						 it.Write(Payload);
					 })
			   .ToArray();
	}
}