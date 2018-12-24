using System;
using System.Threading;
using MechDancer.Common;
using MechDancer.Framework.Net.Modules.TcpConnection;
using MechDancer.Framework.Net.Presets;
using MechDancer.Framework.Net.Protocol;
using MechDancer.Framework.Net.Resources;

namespace UserInterface {
	public static class TestTcp {
		public static void Test() {
			var hub = new RemoteHub("C#");
			hub.Monitor.OpenAll();

			new Thread(() => {
				           while (true) hub.Invoke();
			           }) {IsBackground = true}.Start();

			while (true) {
				var success = hub.Connect
					("kotlin echo server", (byte) TcpCmd.Common,
					 I => {
						 Console.WriteLine("connected framework");
						 while (true) {
							 var sentence = Console.ReadLine();
							 I.Say(sentence.GetBytes());

							 if (sentence == "over") break;
							 I.Listen().GetString().Also(Console.WriteLine);
						 }
					 });

				if (!success) Thread.Sleep(200);
				else break;
			}
		}
	}
}