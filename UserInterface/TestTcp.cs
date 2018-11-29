using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using MechDancer.Framework.Net;
using MechDancer.Framework.Net.Dependency;
using MechDancer.Framework.Net.Remote;
using MechDancer.Framework.Net.Remote.Modules.TcpConnection;
using MechDancer.Framework.Net.Remote.Protocol;
using MechDancer.Framework.Net.Remote.Resources;

namespace UserInterface {
	public static class TestTcp {
		public static void Test() {
			var hub = new RemoteHub("C#");
			hub.OpenAllNetworks();

			Task.Run(() => {
				         while (true) hub.Invoke();
			         });


			var task = Task.Run(async () => {
				                    NetworkStream server;
				                    do {
					                    server = hub.Connect("kotlin echo server", (byte) TcpCmd.Common);
					                    await Task.Delay(200);
				                    } while (server == null);

				                    return server;
			                    });

			using (var I = task.Result) {
				Console.WriteLine("connected framework");
				while (true) {
					var sentence = Console.ReadLine();
					if (sentence == "over") break;

					I.Say(sentence.GetBytes());
					I.Listen().GetString().Also(Console.WriteLine);
				}

				I.Say("over".GetBytes());
			}
		}
	}
}