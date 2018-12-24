using System.Net;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Net.Modules.Multicast;
using MechDancer.Framework.Net.Resources;

namespace MechDancer.Framework.Net.Presets {
	/// <summary>
	///     起搏器
	/// </summary>
	/// <remarks>
	///     为了兼顾灵活性与性能，建议使用起搏器打开网络端口。
	///     使用方法：
	///     1. 其他远程节点绑定所有本地网络接口，但不打开任何接口。
	///     2. 构造起搏器，定时进行触发。
	///     3. 收到起搏器激发包的远程节点将打开正确的网络接口。
	/// </remarks>
	public sealed class Pacemaker {
		private readonly MulticastBroadcaster _broadcaster = new MulticastBroadcaster();
		private readonly MulticastMonitor     _monitor     = new MulticastMonitor();

		/// <summary>
		///     构造器
		/// </summary>
		/// <param name="group">组播地址和端口</param>
		public Pacemaker(IPEndPoint group = null) {
			var scope = new DynamicScope();
			scope.Setup(new Networks());
			scope.Setup(new MulticastSockets(group ?? Default.Group));
			scope.Setup(_monitor);
			scope.Setup(_broadcaster);

			_monitor.OpenAll();
		}

		/// <summary>
		///     重新扫描并打开所有本地网络接口
		/// </summary>
		public void Scan() => _monitor.OpenAll(true);

		/// <summary>
		///     发送激发包
		/// </summary>
		public void Activate() => _broadcaster.Broadcast((byte) UdpCmd.YellAsk);
	}
}