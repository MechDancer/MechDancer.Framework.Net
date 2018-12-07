using System;
using System.Net;
using System.Threading.Tasks;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Net.Modules;
using MechDancer.Framework.Net.Modules.Multicast;
using MechDancer.Framework.Net.Protocol;
using MechDancer.Framework.Net.Resources;

// ReSharper disable RedundantAssignment

namespace UserInterface {
	public static class TestRaw {
		public static void Test() {
			var group    = new Group();
			var receiver = new MulticastReceiver();

			var scope = new DynamicScope()
			   .Also(@this => {
				         @this.Setup(new Name(".Net"));

				         @this.Setup(group);
				         @this.Setup(new GroupMonitor(detected: Console.WriteLine));

				         var networks = new Networks().Also(it => it.Scan());
				         @this.Setup(new MulticastSockets(Address)
					                    .Also(it => {
						                          foreach (var network in networks.View.Keys)
							                          it.Get(network);
					                          }));
				         @this.Setup(new MulticastBroadcaster());
				         @this.Setup(receiver);
				         @this.Setup(new PacketSlicer());

				         @this.Setup(new CommonUdpServer((_, pack) => Console.WriteLine(pack.GetString())));
			         });

			async Task Display(TimeSpan timeSpan) {
				Console.Write("members: [");
				foreach (var member in group[timeSpan])
					Console.Write($" {member}");
				Console.WriteLine(" ]");
				await Task.Delay(timeSpan);
			}

			Task.Run(async () => {
				         while (true) {
					         await Display(TimeSpan.FromSeconds(1));
				         }
			         });

			while (true) receiver.Invoke().Also(Console.WriteLine);
		}

		private static readonly IPEndPoint Address
			= new IPEndPoint(IPAddress.Parse("233.33.33.33"), 23333);
	}
}