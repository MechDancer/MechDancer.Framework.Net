using System;
using System.Threading;
using MechDancer.Framework.Net.Presets;

namespace UserInterface {
	internal static class Program {
		private static void Main() {
			var probe = new Probe();
			new Thread(() => {
				while (true) probe.Invoke();
			}).Start();

			while (true) {
				Console.ReadKey();
				Console.WriteLine($"{DateTime.Now}:");
				foreach (var (name, (time, address)) in probe.View) Console.WriteLine($"{name}: {time}, {address}");
				Console.WriteLine("___");
			}
		}
	}
}
