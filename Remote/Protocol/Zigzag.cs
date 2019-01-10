using System.IO;
using MechDancer.Common;

namespace MechDancer.Framework.Net.Protocol {
	public static class Functions {
		/// <summary>
		///     在流上编码变长整数
		/// </summary>
		/// <param name="receiver">流</param>
		/// <param name="num">数字</param>
		/// <param name="signed">是否按有符号编码</param>
		/// <typeparam name="T">流实现类型</typeparam>
		/// <returns>流</returns>
		public static T WriteZigzag<T>(
			this T receiver,
			long   num,
			bool   signed
		) where T : Stream {
			var temp = (ulong) (signed ? (num << 1) ^ (num >> 63) : num);

			while (true)
				if (temp > 0x7f) {
					receiver.WriteByte((byte) (temp | 0x80));
					temp >>= 7;
				} else {
					receiver.WriteByte((byte) temp);
					return receiver;
				}
		}

		public static long ReadZigzag(
			this Stream receiver,
			bool        signed
		) => new MemoryStream(10)
			.Also(stream => {
					  int b;
					  do {
						  b = receiver.ReadByte();
						  stream.WriteByte((byte) b);
					  } while (b > 0x7f);
				  })
			.ToArray()
			.Zigzag(signed);

		public static byte[] Zigzag(
			this long receiver,
			bool      signed
		) => new MemoryStream(10)
			.WriteZigzag(receiver, signed)
			.ToArray();

		public static long Zigzag(
			this byte[] receiver,
			bool        signed
		) {
			var acc = 0UL;
			for (var i = receiver.Length - 1; i >= 0; --i)
				acc = (acc << 7) | ((ulong) receiver[i] & 0x7f);
			return signed ? (long) (acc >> 1) ^ -((long) acc & 1) : (long) acc;
		}

		/// <summary>
		///     先向流中写入长度再写入数据
		/// </summary>
		/// <param name="receiver">流</param>
		/// <param name="payload">数据</param>
		/// <typeparam name="T">流实现类型</typeparam>
		/// <returns>流本身</returns>
		public static T WriteWithLength<T>(this T receiver, byte[] payload) where T : Stream
			=> receiver.Also(it => {
								 it.WriteZigzag(payload.Length, false);
								 it.Write(payload);
							 });

		/// <summary>
		///     先从流中读取长度，再读取指定长度内容
		/// </summary>
		/// <param name="receiver">流</param>
		/// <returns>读到的内容</returns>
		public static byte[] ReadWithLength(this Stream receiver)
			=> receiver.WaitNBytes((int) receiver.ReadZigzag(false));
	}
}