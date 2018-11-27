using System;
using System.Threading.Tasks;
using MechDancer.Framework.Net;
using MechDancer.Framework.Net.Remote;
using MechDancer.Framework.Net.Remote.Modules.TcpConnection;
using MechDancer.Framework.Net.Remote.Protocol;
using MechDancer.Framework.Net.Remote.Resources;
using static MechDancer.Framework.Net.Dependency.Functions;

namespace UserInterface {
	internal static class Program {
		private static void Main() {
			var hub = new RemoteHub("client");
			hub.OpenAllNetworks();

			Task.Run(() => {
				         while (true) hub.Invoke();
			         });

			var addresses    = hub.Hub.Must<Addresses>();
			var synchronizer = hub.Hub.Must<PortMonitor>();

			async Task Asking() {
				while (addresses["framework"] == null) {
					synchronizer.Ask("framework");
					await Task.Delay(1000);
				}
			}

			Task.Run(Asking).Wait();

			using (var I = hub.Connect("framework", (byte) TcpCmd.Common)) {
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