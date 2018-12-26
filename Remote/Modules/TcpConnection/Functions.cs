using System.Net.Sockets;
using MechDancer.Common;
using MechDancer.Framework.Net.Protocol;

namespace MechDancer.Framework.Net.Modules.TcpConnection {
	public static class Functions {
		public static void Say(this NetworkStream receiver, byte cmd) {
			receiver.WriteByte(cmd);
		}

		public static void Say(this NetworkStream receiver, byte[] payload) {
			receiver.WriteWithLength(payload);
		}

		public static void Say(this NetworkStream receiver, string text) {
			receiver.WriteEnd(text);
		}

		public static byte ListenCommand(this NetworkStream receiver) {
			return (byte) receiver.ReadByte();
		}

		public static string ListenString(this NetworkStream receiver) {
			return receiver.ReadEnd();
		}

		public static byte[] Listen(this NetworkStream receiver) {
			return receiver.ReadWithLength();
		}
	}
}