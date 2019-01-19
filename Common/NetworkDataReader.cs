using System;
using System.IO;

namespace MechDancer.Common {
	/// <summary>
	///     网络字节序数据读取
	/// </summary>
	public class NetworkDataReader {
		private readonly Stream _stream;

		public NetworkDataReader(Stream stream) => _stream = stream;

		public void Dispose() => _stream.Dispose();

		public byte ReadByte()
			=> (byte) _stream.ReadByte();

		public sbyte ReadSByte()
			=> (sbyte) _stream.ReadByte();

		public short ReadShort()
			=> BitConverter.ToInt16(_stream.WaitReversed(sizeof(short)), 0);

		public ushort ReadUShort()
			=> BitConverter.ToUInt16(_stream.WaitReversed(sizeof(ushort)), 0);

		public int ReadInt()
			=> BitConverter.ToInt32(_stream.WaitReversed(sizeof(int)), 0);

		public uint ReadUInt()
			=> BitConverter.ToUInt32(_stream.WaitReversed(sizeof(uint)), 0);

		public long ReadLong()
			=> BitConverter.ToInt64(_stream.WaitReversed(sizeof(long)), 0);

		public ulong ReadULong()
			=> BitConverter.ToUInt64(_stream.WaitReversed(sizeof(ulong)), 0);

		public float ReadFloat()
			=> BitConverter.ToSingle(_stream.WaitReversed(sizeof(float)), 0);

		public double ReadDouble()
			=> BitConverter.ToDouble(_stream.WaitReversed(sizeof(double)), 0);
	}
}
