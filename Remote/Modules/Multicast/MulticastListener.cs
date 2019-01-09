using System;
using System.Collections.Generic;
using MechDancer.Framework.Net.Protocol;

namespace MechDancer.Framework.Net.Modules.Multicast {
	/// <inheritdoc />
	/// <summary>
	///     组播监听者的便捷实现
	/// </summary>
	public sealed class MulticastListener : IMulticastListener {
		private readonly Action<RemotePacket> _callback;

		/// <summary>
		///     构造器
		/// </summary>
		/// <param name="callback">回调函数</param>
		/// <param name="interest">关注指令</param>
		public MulticastListener(
			Action<RemotePacket> callback,
			params byte[]        interest
		) {
			Interest  = interest;
			_callback = callback;
		}

		public IReadOnlyCollection<byte> Interest { get; }

		public void Process(RemotePacket remotePacket) => _callback(remotePacket);

		public override bool Equals(object obj) => ReferenceEquals(this, obj);
		public override int  GetHashCode()      => typeof(MulticastListener).GetHashCode();
	}
}