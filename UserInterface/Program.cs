using System;
using System.Net;
using System.Threading;
using MechDancer.Framework.Net;
using MechDancer.Framework.Net.Resources;

namespace UserInterface {
	internal static class Program {
		private static readonly IPEndPoint IpEndPoint
			= new IPEndPoint(IPAddress.Parse("233.33.33.33"), 23333);
//
//		private static void Main() {
//			var temp1 = new UdpMulticastClient(IpEndPoint, null);
//            temp1.Socket.Blocking = false;
//            while (temp1.Socket.Available == 0) ;
//			temp1.ReceiveFrom(new byte[1024], out _);
//		}

		private static void Main() {
			var networks = new Networks();
			foreach (var pair in networks.View.Values) {
				var client = new UdpMulticastClient(IpEndPoint, pair.Address);
				new Thread(() => {
					           Console.WriteLine(client.Socket.LocalEndPoint);
					           while (true) {
						           client.ReceiveFrom(new byte[2048], out var temp);
						           Console.WriteLine(temp);
					           }
				           }).Start();
			}
		}
	}
}