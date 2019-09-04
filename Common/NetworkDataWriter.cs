using System;
using System.IO;

namespace MechDancer.Common {
	/// <summary>
	/// 网络字节序数据写入
	/// </summary>
	public class NetworkDataWriter : IDisposable {
		private readonly Stream _stream;

		public NetworkDataWriter(Stream stream) => _stream = stream;

		public void Dispose() => _stream.Dispose();

        /// <summary>
        /// 向流中写入无符号字节
        /// </summary>
        /// <param name="value">数据</param>
		public void Write(byte value)
			=> _stream.WriteByte(value);

        /// <summary>
        /// 向流中写入有符号字节
        /// </summary>
        /// <param name="value">数据</param>
        public void Write(sbyte value)
			=> _stream.WriteByte((byte) value);

        /// <summary>
        /// 向流中写入有符号短整型
        /// </summary>
        /// <param name="value">数据</param>
        public void Write(short value)
			=> _stream.WriteReversed(BitConverter.GetBytes(value));

        /// <summary>
        /// 向流中写入无符号短整型
        /// </summary>
        /// <param name="value">数据</param>
        public void Write(ushort value)
			=> _stream.WriteReversed(BitConverter.GetBytes(value));

        /// <summary>
        /// 向流中写入有符号整型
        /// </summary>
        /// <param name="value">数据</param>
        public void Write(int value)
			=> _stream.WriteReversed(BitConverter.GetBytes(value));

        /// <summary>
        /// 向流中写入无符号整型
        /// </summary>
        /// <param name="value">数据</param>
        public void Write(uint value)
			=> _stream.WriteReversed(BitConverter.GetBytes(value));

        /// <summary>
        /// 向流中写入有符号长整型
        /// </summary>
        /// <param name="value">数据</param>
        public void Write(long value)
			=> _stream.WriteReversed(BitConverter.GetBytes(value));

        /// <summary>
        /// 向流中写入无符号长整型
        /// </summary>
        /// <param name="value">数据</param>
        public void Write(ulong value)
			=> _stream.WriteReversed(BitConverter.GetBytes(value));

        /// <summary>
        /// 向流中写入单精度浮点
        /// </summary>
        /// <param name="value">数据</param>
        public void Write(float value)
			=> _stream.WriteReversed(BitConverter.GetBytes(value));

        /// <summary>
        /// 向流中写入双精度浮点
        /// </summary>
        /// <param name="value">数据</param>
        public void Write(double value)
			=> _stream.WriteReversed(BitConverter.GetBytes(value));
	}
}
