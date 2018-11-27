using System;
using System.IO;
using System.Text;

namespace MechDancer.Framework.Net.Remote.Protocol {
	public static partial class Functions {
		/// <summary>
		/// 	按范围拷贝数组
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
		   .Also(it => receiver.CopyTo(it, 0));

		/// <summary>
		/// 	向流写入一个字符数组
		/// </summary>
		/// <param name="receiver">流</param>
		/// <param name="bytes">字符数组</param>
		/// <typeparam name="T">流实现类型</typeparam>
		/// <returns>流</returns>
		public static T Write<T>(this T receiver, byte[] bytes)
			where T : Stream =>
			receiver.Also(it => it.Write(bytes, 0, bytes.Length));

		/// <summary>
		/// 	把一个字节数组按相反的顺序写入流
		/// </summary>
		/// <param name="receiver">目标流</param>
		/// <param name="bytes">字节数组</param>
		/// <param name="index">起始位置</param>
		/// <param name="length">写入的长度</param>
		/// <typeparam name="T">流具体类型</typeparam>
		/// <returns>所在流</returns>
		public static T WriteReversed<T>(
			this T receiver,
			byte[] bytes,
			int    index  = 0,
			int    length = int.MaxValue
		) where T : Stream =>
			receiver.Also
				(it => {
					 index += Math.Min(length, bytes.Length - index);
					 while (index-- > 0) it.WriteByte(bytes[index]);
				 });

		/// <summary>
		/// 	从输入流阻塞接收 n 个字节数据，或直到流关闭。
		/// 	函数会直接打开等于目标长度的缓冲区，因此不要用于实现尽量读取的功能。
		/// </summary>
		/// <param name="receiver"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		public static byte[] WaitNBytes(this Stream receiver, uint n) =>
			new MemoryStream(new byte[n]).Let
				(buffer => {
					 for (var i = 0; i < n; ++i) {
						 var temp = receiver.ReadByte();
						 if (temp > 0) buffer.WriteByte((byte) temp);
						 else return buffer.GetBuffer().CopyRange(0, i);
					 }

					 return buffer.GetBuffer();
				 });

		/// <summary>
		/// 	从输入流阻塞接收 n 个字节数据，并将数组按相反的方向读出。
		/// </summary>
		/// <param name="receiver"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		public static byte[] WaitReversed(this Stream receiver, uint n) =>
			new byte[n].Also
				(buffer => {
					 while (--n > 0) {
						 var temp = receiver.ReadByte();
						 if (temp > 0) buffer[n] = (byte) temp;
						 else throw new IOException("stream is already end");
					 }
				 });

		/// <summary>
		/// 	字节数组转字符串
		/// </summary>
		/// <param name="receiver"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static string GetString(this byte[] receiver, Encoding encoding = null) =>
			(encoding ?? Encoding.Default).GetString(receiver);

		/// <summary>
		/// 	字符串转字节数组
		/// </summary>
		/// <param name="receiver"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static byte[] GetBytes(this string receiver, Encoding encoding = null) =>
			(encoding ?? Encoding.Default).GetBytes(receiver);

		/// <summary>
		/// 	向流中写入字符串，再写入结尾
		/// </summary>
		/// <param name="receiver">流</param>
		/// <param name="text">字符串</param>
		/// <typeparam name="T">流实现类型</typeparam>
		/// <returns>流</returns>
		public static T WriteEnd<T>(this T receiver, string text)
			where T : Stream =>
			receiver.Also
				(it => {
					 it.Write(text.GetBytes());
					 it.WriteByte(0);
				 });

		/// <summary>
		/// 	从流读取一个带结尾的字符串
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

		/// <summary>
		/// 	从流中读取所有数据
		/// </summary>
		/// <param name="receiver"></param>
		/// <returns></returns>
		public static byte[] ReadRest(this Stream receiver) {
			var buffer = new MemoryStream();
			while (true) {
				var b = receiver.ReadByte();
				if (b == -1) return buffer.ToArray();
				buffer.WriteByte((byte) b);
			}
		}

		public static T WriteWithLength<T>(this T receiver, byte[] payload)
			where T : Stream =>
			receiver.Also
				(it => {
					 it.WriteZigzag(payload.Length, false);
					 it.Write(payload);
				 });

		public static byte[] ReadWithLength(this Stream receiver) =>
			receiver.WaitNBytes((uint) receiver.ReadZigzag(false));
	}
}