using System;
using System.Threading;
using System.Threading.Tasks;
using MechDancer.Framework.Net.Presets;

namespace UserInterface {
	public static class Examples {
		/// <summary>
		///     测试起搏器
		/// </summary>
		public static async Task UsePacemaker() {
			var pacemaker = new Pacemaker();
			while (true) {
				pacemaker.Activate();
				await Task.Delay(1000);
			}
		}

		/// <summary>
		///     测试探针
		/// </summary>
		public static void UseProbe() {
			var probe = new Probe();
			new Thread(() => {
				           while (true) probe.Invoke();
			           }).Start();

			while (true) {
				Console.ReadKey();
				Console.WriteLine($"{DateTime.Now}:");
				foreach (var (name, (time, address)) in probe.View)
					Console.WriteLine($"{name}: {time}, {address}");
				Console.WriteLine("___");
			}
		}
	}
}