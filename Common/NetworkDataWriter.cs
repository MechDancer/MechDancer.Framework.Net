using System;
using System.IO;

namespace MechDancer.Common {
	/// <summary>
	///     网络字节序数据写入
	/// </summary>
	public class NetworkDataWriter : IDisposable {
		private readonly Stream _stream;

		public NetworkDataWriter(Stream stream) => _stream = stream;

		public void Dispose() => _stream.Dispose();

		public void Write(byte value)
			=> _stream.WriteByte(value);

		public void Write(sbyte value)
			=> _stream.WriteByte((byte) value);

		public void Write(short value)
			=> _stream.WriteReversed(BitConverter.GetBytes(value));

		public void Write(ushort value)
			=> _stream.WriteReversed(BitConverter.GetBytes(value));

		public void Write(int value)
			=> _stream.WriteReversed(BitConverter.GetBytes(value));

		public void Write(uint value)
			=> _stream.WriteReversed(BitConverter.GetBytes(value));

		public void Write(long value)
			=> _stream.WriteReversed(BitConverter.GetBytes(value));

		public void Write(ulong value)
			=> _stream.WriteReversed(BitConverter.GetBytes(value));

		public void Write(float value)
			=> _stream.WriteReversed(BitConverter.GetBytes(value));

		public void Write(double value)
			=> _stream.WriteReversed(BitConverter.GetBytes(value));
	}
}
