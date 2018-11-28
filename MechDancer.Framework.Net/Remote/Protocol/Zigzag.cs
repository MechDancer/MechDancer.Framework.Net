using System.IO;

namespace MechDancer.Framework.Net.Remote.Protocol {
	public static partial class Functions {
		/// <summary>
		/// 	在流上编码变长整数
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

			while (true) {
				if (temp > 0x7f) {
					receiver.WriteByte((byte) (temp | 0x80));
					temp >>= 7;
				}
				else {
					receiver.WriteByte((byte) temp);
					return receiver;
				}
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
	}
}