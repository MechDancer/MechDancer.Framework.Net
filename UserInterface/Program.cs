using System;
using System.Net;
using MechDancer.Framework.Net;
using MechDancer.Framework.Net.Remote.Modules.Multicast;
using MechDancer.Framework.Net.Remote.Resources;
using static MechDancer.Framework.Net.Dependency.Functions;

namespace UserInterface {
	internal static class Program {
		private static void Main() {
			TestTcp.Test();
		}
	}
}