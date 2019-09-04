using System;
using System.IO;

namespace MechDancer.Common {
	/// <summary>
	/// 网络字节序数据读取
	/// </summary>
	public class NetworkDataReader {
		private readonly Stream _stream;

		public NetworkDataReader(Stream stream) => _stream = stream;

		public void Dispose() => _stream.Dispose();

        /// <summary>
        /// 从流中读取无符号字节
        /// </summary>
        /// <returns>字节</returns>
		public byte ReadByte()
			=> (byte) _stream.ReadByte();

        /// <summary>
        /// 从流中读取有符号字节
        /// </summary>
        /// <returns>字节</returns>
        public sbyte ReadSByte()
			=> (sbyte) _stream.ReadByte();

        /// <summary>
        /// 从流中读取有符号短整型
        /// </summary>
        /// <returns>短整型</returns>
        public short ReadShort()
			=> BitConverter.ToInt16(_stream.WaitReversed(sizeof(short)), 0);

        /// <summary>
        /// 从流中读取无符号短整型
        /// </summary>
        /// <returns>短整型</returns>
        public ushort ReadUShort()
			=> BitConverter.ToUInt16(_stream.WaitReversed(sizeof(ushort)), 0);

        /// <summary>
        /// 从流中读取有符号整型
        /// </summary>
        /// <returns>整型</returns>
        public int ReadInt()
			=> BitConverter.ToInt32(_stream.WaitReversed(sizeof(int)), 0);

        /// <summary>
        /// 从流中读取无符号整型
        /// </summary>
        /// <returns>整型</returns>
        public uint ReadUInt()
			=> BitConverter.ToUInt32(_stream.WaitReversed(sizeof(uint)), 0);

        /// <summary>
        /// 从流中读取有符号长整型
        /// </summary>
        /// <returns>长整型</returns>
        public long ReadLong()
			=> BitConverter.ToInt64(_stream.WaitReversed(sizeof(long)), 0);

        /// <summary>
        /// 从流中读取无符号长整型
        /// </summary>
        /// <returns>长整型</returns>
        public ulong ReadULong()
			=> BitConverter.ToUInt64(_stream.WaitReversed(sizeof(ulong)), 0);

        /// <summary>
        /// 从流中读取单精度浮点
        /// </summary>
        /// <returns>浮点</returns>
        public float ReadFloat()
			=> BitConverter.ToSingle(_stream.WaitReversed(sizeof(float)), 0);

        /// <summary>
        /// 从流中读取双精度浮点
        /// </summary>
        /// <returns>浮点</returns>
        public double ReadDouble()
			=> BitConverter.ToDouble(_stream.WaitReversed(sizeof(double)), 0);
	}
}
