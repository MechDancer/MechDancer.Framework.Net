using System;
using System.Threading;
using MechDancer.Common;
using MechDancer.Framework.Net.Modules.Multicast;
using MechDancer.Framework.Net.Modules.TcpConnection;
using MechDancer.Framework.Net.Presets;
using MechDancer.Framework.Net.Resources;

namespace UserInterface {
	public static class TestTcp {
		public static void TestClient(string serverName) {
			var hub = new RemoteHub("C#");
			hub.Monitor.OpenAll();

			new Thread(() => {
						   while (true) hub.Invoke();
					   }) {IsBackground = true}.Start();

			while (true) {
				var success = hub.Connect
					(serverName, (byte) TcpCmd.Common,
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

        public static void TestServer() {
            var hub = new RemoteHub("C# Server",
                additions: new ConnectionListener
                     (interest: (byte)TcpCmd.Common,
                      func: (name, I) => {
                          Console.WriteLine("connected: {0}", name);
                          while (true) {
                              var sentence = I.Listen().GetString();
                              if (sentence == "over") break;
                              Console.WriteLine("server heared: {0}", sentence);
                              I.Say(sentence.GetBytes());
                          }
                          Console.WriteLine("server heared over to break");
                      }));
            hub.Yell();

            new Thread(() => {
                while (true) hub.Invoke();
            }).Start();

            while (true) hub.Accept();
        }
	}
}