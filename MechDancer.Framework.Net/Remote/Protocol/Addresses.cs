using System;
using System.IO;
using System.Net;

namespace MechDancer.Framework.Net.Remote.Protocol {
	public static partial class Functions {
		/// <summary>
		/// 	从字节数组恢复套接字地址
		/// </summary>
		/// <param name="bytes">字节数组</param>
		/// <returns>套接字地址</returns>
		public static IPEndPoint BuildIpEndPoint(byte[] bytes) =>
			new MemoryStream(bytes).ReadIpEndPoint();

		/// <summary>
		/// 	套接字地址打包到字节数组
		/// </summary>
		/// <param name="receiver">套接字地址</param>
		/// <returns>字节数组</returns>
		public static byte[] GetBytes(this IPEndPoint receiver) =>
			new MemoryStream(new byte[8]).Write(receiver).GetBuffer();

		/// <summary>
		/// 	从流读取一个套接字地址
		/// </summary>
		/// <param name="receiver">流</param>
		/// <returns>套接字地址</returns>
		public static IPEndPoint ReadIpEndPoint(this Stream receiver) =>
			new IPEndPoint(new IPAddress(receiver.WaitNBytes(4)),
			               BitConverter.ToInt32(receiver.WaitReversed(4)));

		/// <summary>
		/// 	向流写入一个套接字地址
		/// </summary>
		/// <param name="receiver">流</param>
		/// <param name="endPoint">套接字地址</param>
		/// <typeparam name="T">流实现类型</typeparam>
		/// <returns>流</returns>
		public static T Write<T>(this T receiver, IPEndPoint endPoint)
			where T : Stream {
			receiver.Write(endPoint.Address.GetAddressBytes());
			return receiver.WriteReversed(BitConverter.GetBytes(endPoint.Port));
		}
	}
}