using System;
using System.IO;

namespace MechDancer.Framework.Net.Remote.Protocol {
	public static partial class Functions {
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
		) where T : Stream {
			index += Math.Min(length, bytes.Length - index);
			while (index-- > 0) receiver.WriteByte(bytes[index]);
			return receiver;
		}

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
		) {
			var buffer = new T[Math.Min(end, receiver.Length) - begin];
			receiver.CopyTo(buffer, 0);
			return buffer;
		}

		/// <summary>
		/// 	从输入流阻塞接收 n 个字节数据，或直到流关闭。
		/// 	函数会直接打开等于目标长度的缓冲区，因此不要用于实现尽量读取的功能。
		/// </summary>
		/// <param name="receiver"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		public static byte[] WaitNBytes(this Stream receiver, uint n) {
			var buffer = new MemoryStream(new byte[n]);

			for (var i = 0; i < n; ++i) {
				var temp = receiver.ReadByte();
				if (temp > 0) buffer.WriteByte((byte) temp);
				else return buffer.GetBuffer().CopyRange(0, i);
			}

			return buffer.GetBuffer();
		}

		/// <summary>
		/// 	从输入流阻塞接收 n 个字节数据，并将数组按相反的方向读出。
		/// </summary>
		/// <param name="receiver"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		public static byte[] WaitReversed(this Stream receiver, uint n) {
			var buffer = new byte[n];

			while (--n > 0) {
				var temp = receiver.ReadByte();
				if (temp > 0) buffer[n] = (byte) temp;
				else throw new IOException("stream is already end");
			}

			return buffer;
		}
	}
}