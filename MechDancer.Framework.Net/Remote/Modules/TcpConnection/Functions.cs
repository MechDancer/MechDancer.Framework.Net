using System;
using System.Net.Sockets;
using MechDancer.Framework.Net.Remote.Protocol;

namespace MechDancer.Framework.Net.Remote.Modules.TcpConnection {
	public static class Functions {
		public static void Say(this NetworkStream receiver, byte cmd) =>
			receiver.WriteByte(cmd);

		public static void Say(this NetworkStream receiver, byte[] payload) =>
			receiver.WriteWithLength(payload);

		public static T Listen<T>(this NetworkStream receiver, Func<byte, T> block) =>
			block.Invoke((byte) receiver.ReadByte());

		public static byte[] Listen(this NetworkStream receiver) =>
			receiver.ReadWithLength();
	}
}