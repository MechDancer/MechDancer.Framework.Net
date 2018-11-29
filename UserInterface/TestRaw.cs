using System;
using System.Net;
using System.Threading.Tasks;
using MechDancer.Framework.Net;
using MechDancer.Framework.Net.Remote.Modules;
using MechDancer.Framework.Net.Remote.Modules.Multicast;
using MechDancer.Framework.Net.Remote.Resources;
using static MechDancer.Framework.Net.Dependency.Functions;

// ReSharper disable RedundantAssignment

namespace UserInterface {
	public static class TestRaw {
		public static void Test() {
			var scope = Scope
				(@this => {
					 @this += new Name(".Net");

					 @this += new Group();
					 @this += new GroupMonitor(Console.WriteLine);

					 var networks = new Networks().Also(it => it.Scan());
					 @this += new MulticastSockets(Address)
						.Also(it => {
							      foreach (var network in networks.View.Keys)
								      it.Get(network);
						      });
					 @this += new MulticastBroadcaster();
					 @this += new MulticastReceiver();
				 });

			var group    = scope.Dependencies.Must<Group>();
			var receiver = scope.Dependencies.Must<MulticastReceiver>();

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

			while (true) receiver.Invoke();
		}

		private static readonly IPEndPoint Address
			= new IPEndPoint(IPAddress.Parse("233.33.33.33"), 23333);
	}
}