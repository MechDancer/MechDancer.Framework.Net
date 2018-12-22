using System.Net;
using MechDancer.Framework.Net;

namespace UserInterface {
	internal static class Program {
		private static readonly IPEndPoint IpEndPoint
			= new IPEndPoint(IPAddress.Parse("233.33.33.33"), 23333);

		private static void Main() {
			var temp1 = new UdpMulticastClient(IpEndPoint, null);
			temp1.Socket.Blocking = false;
			while (temp1.Socket.Available == 0) ;
			temp1.ReceiveFrom(new byte[1024], out _);
		}
	}
}