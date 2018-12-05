using System.Threading;

namespace UserInterface {
	internal static class Program {
		private static void Main() {
			TestTcp.Test();
//			new Thread(TestSlice.TestReceive).Start();
//			TestSlice.TestSend();
//			Thread.Sleep(1000);
		}
	}
}