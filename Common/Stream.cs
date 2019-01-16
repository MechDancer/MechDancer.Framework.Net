using System;
using System.IO;
using System.Text;

namespace MechDancer.Common {
	public static partial class Extensions {
		/// <summary>
		///     按范围拷贝数组
		/// </summary>
		/// <param name="receiver">原数组</param>
		/// <param name="begin">起始坐标（包括）</param>
		/// <param name="end">结束坐标（不包括）</param>
		/// <typeparam name="T">数组类型，引用类型浅拷贝</typeparam>
		/// <returns>新数组</returns>
		public static T[] CopyRange<T>(
			this T[] receiver,
			int      begin = 0,
			int      end   = int.MaxValue
		) => new T[Math.Min(end, receiver.Length) - begin]
		   .Also(it => Array.Copy(receiver, begin, it, 0, it.Length));

		/// <summary>
		///     向流写入一个字符数组
		/// </summary>
		/// <param name="receiver">流</param>
		/// <param name="bytes">字符数组</param>
		/// <returns>流</returns>
		public static void Write(this Stream receiver, byte[] bytes)
			=> receiver.Write(bytes, 0, bytes.Length);

		/// <summary>
		///     把一个字节数组按相反的顺序写入流
		/// </summary>
		/// <param name="receiver">目标流</param>
		/// <param name="bytes">字节数组</param>
		/// <param name="index">起始位置</param>
		/// <param name="length">写入的长度</param>
		/// <returns>所在流</returns>
		public static void WriteReversed(
			this Stream receiver,
			byte[]      bytes,
			int         index  = 0,
			int         length = int.MaxValue
		) {
			index += Math.Min(length, bytes.Length - index);
			while (index-- > 0) receiver.WriteByte(bytes[index]);
		}

		/// <summary>
		///     从输入流阻塞接收 n 个字节数据，或直到流关闭。
		///     函数会直接打开等于目标长度的缓冲区，因此不要用于实现尽量读取的功能。
		/// </summary>
		/// <param name="receiver">输入流</param>
		/// <param name="n">数量</param>
		/// <returns>缓冲内存块</returns>
		public static byte[] WaitNBytes(this Stream receiver, int n) =>
			new MemoryStream(n).Let
				(buffer => {
					 for (var i = 0; i < n; ++i) {
						 var temp = receiver.ReadByte();
						 if (temp > 0) buffer.WriteByte((byte) temp);
						 else return buffer.GetBuffer().CopyRange(0, i);
					 }

					 return buffer.GetBuffer();
				 });

		/// <summary>
		///     从输入流阻塞接收 n 个字节数据，并将数组按相反的方向读出。
		/// </summary>
		/// <param name="receiver">输入流</param>
		/// <param name="n">数量</param>
		/// <returns>内存块</returns>
		public static byte[] WaitReversed(this Stream receiver, uint n) {
			var buffer = new byte[n];

			while (n-- > 0) {
				var temp = receiver.ReadByte();
				if (temp >= 0) buffer[n] = (byte) temp;
				else return buffer.CopyRange((int) n + 1, buffer.Length);
			}

			return buffer;
		}

		/// <summary>
		///     从流中读取所有数据
		/// </summary>
		/// <param name="receiver">字节流</param>
		/// <returns>剩余数据</returns>
		public static byte[] ReadRest(this Stream receiver) {
			var buffer = new MemoryStream();
			while (true) {
				var b = receiver.ReadByte();
				if (b == -1) return buffer.ToArray();
				buffer.WriteByte((byte) b);
			}
		}

		/// <summary>
		///     计算内存流剩余空间
		/// </summary>
		/// <param name="receiver">内存流</param>
		/// <returns>剩余空间长度</returns>
		public static long Available(this MemoryStream receiver)
			=> receiver.Capacity - receiver.Position;

		/// <summary>
		///     向流中写入字符串，再写入结尾
		/// </summary>
		/// <param name="receiver">流</param>
		/// <param name="text">字符串</param>
		/// <returns>流</returns>
		public static void WriteEnd(this Stream receiver, string text)
			=> receiver.Also(it => {
								 it.Write(text.GetBytes());
								 it.WriteByte(0);
							 });

		/// <summary>
		///     从流读取一个带结尾的字符串
		/// </summary>
		/// <param name="receiver">流</param>
		/// <returns>字符串</returns>
		public static string ReadEnd(this Stream receiver) {
			var buffer = new MemoryStream(1);
			while (true) {
				var b = receiver.ReadByte();
				switch (b) {
					case -1:
					case 0:
						return buffer.ToArray().GetString();
					default:
						buffer.WriteByte((byte) b);
						break;
				}
			}
		}

		#region String Encode

		/// <summary>
		///     字节数组转字符串
		/// </summary>
		/// <param name="receiver">字节数组</param>
		/// <param name="encoding">字符串编码</param>
		/// <returns>字符串</returns>
		public static string GetString(this byte[] receiver, Encoding encoding = null) =>
			(encoding ?? Encoding.Default).GetString(receiver);

		/// <summary>
		///     字符串转字节数组
		/// </summary>
		/// <param name="receiver">字符串</param>
		/// <param name="encoding">字符串编码</param>
		/// <returns>字节数组</returns>
		public static byte[] GetBytes(this string receiver, Encoding encoding = null) =>
			(encoding ?? Encoding.Default).GetBytes(receiver);

		#endregion

		#region Data Encode

		public static void Write(this Stream receiver, sbyte value)
			=> receiver.WriteByte((byte) value);

		public static void Write(this Stream receiver, short value)
			=> receiver.WriteReversed(BitConverter.GetBytes(value));

		public static void Write(this Stream receiver, ushort value)
			=> receiver.WriteReversed(BitConverter.GetBytes(value));

		public static void Write(this Stream receiver, int value)
			=> receiver.WriteReversed(BitConverter.GetBytes(value));

		public static void Write(this Stream receiver, uint value)
			=> receiver.WriteReversed(BitConverter.GetBytes(value));

		public static void Write(this Stream receiver, long value)
			=> receiver.WriteReversed(BitConverter.GetBytes(value));

		public static void Write(this Stream receiver, ulong value)
			=> receiver.WriteReversed(BitConverter.GetBytes(value));

		public static void Write(this Stream receiver, float value)
			=> receiver.WriteReversed(BitConverter.GetBytes(value));

		public static void Write(this Stream receiver, double value)
			=> receiver.WriteReversed(BitConverter.GetBytes(value));

		public static sbyte ReadSByte(this Stream receiver)
			=> (sbyte) receiver.ReadByte();

		public static short ReadShort(this Stream stream)
			=> BitConverter.ToInt16(stream.WaitReversed(sizeof(short)), 0);

		public static ushort ReadUShort(this Stream stream)
			=> BitConverter.ToUInt16(stream.WaitReversed(sizeof(ushort)), 0);

		public static int ReadInt(this Stream stream)
			=> BitConverter.ToInt32(stream.WaitReversed(sizeof(int)), 0);

		public static uint ReadUInt(this Stream stream)
			=> BitConverter.ToUInt32(stream.WaitReversed(sizeof(uint)), 0);

		public static long ReadLong(this Stream stream)
			=> BitConverter.ToInt64(stream.WaitReversed(sizeof(long)), 0);

		public static ulong ReadULong(this Stream stream)
			=> BitConverter.ToUInt64(stream.WaitReversed(sizeof(ulong)), 0);

		public static float ReadFloat(this Stream stream)
			=> BitConverter.ToSingle(stream.WaitReversed(sizeof(float)), 0);

		public static double ReadDouble(this Stream stream)
			=> BitConverter.ToDouble(stream.WaitReversed(sizeof(double)), 0);

		#endregion
	}
}