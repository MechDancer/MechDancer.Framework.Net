using System.Net;

namespace MechDancer.Framework.Net.Presets {
	/// <summary>
	///     常用默认值
	/// </summary>
	internal static class Default {
		/// <summary>
		///     默认组播地址和端口
		/// </summary>
		public static readonly IPEndPoint Group
			= new IPEndPoint(IPAddress.Parse("233.33.33.33"), 23333);
	}
}