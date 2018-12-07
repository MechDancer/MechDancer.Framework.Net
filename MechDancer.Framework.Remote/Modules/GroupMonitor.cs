using System;
using System.Collections.Generic;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Dependency.UniqueComponent;
using MechDancer.Framework.Net.Modules.Multicast;
using MechDancer.Framework.Net.Protocol;
using MechDancer.Framework.Net.Resources;

namespace MechDancer.Framework.Net.Modules {
	/// <summary>
	/// 	组成员的管理器
	/// </summary>
	/// <remarks>
	///		发现新成员时将调用回调函数
	///     从未出现过的成员或离线时间超过超时时间的成员视作新成员
	/// </remarks>
	public sealed class GroupMonitor : UniqueComponent<GroupMonitor>,
	                                   IDependent,
	                                   IMulticastListener {
		private readonly UniqueDependencies _dependencies = new UniqueDependencies();

		private readonly UniqueDependency<MulticastBroadcaster> _broadcaster;
		private readonly UniqueDependency<Group>                _group;
		private readonly Action<string>                         _detected;
		private readonly TimeSpan                               _timeout;

		/// <summary>
		/// 	构造器
		/// </summary>
		/// <param name="timeout">超时时间，超过此时间重新出现的成员视作新成员</param>
		/// <param name="detected">发现新成员的回调函数</param>
		public GroupMonitor(TimeSpan? timeout = null, Action<string> detected = null) {
			_timeout     = timeout ?? TimeSpan.MaxValue;
			_detected    = detected;
			_group       = _dependencies.BuildDependency<Group>();
			_broadcaster = _dependencies.BuildDependency<MulticastBroadcaster>();
		}

		public bool Sync(IComponent dependency) => _dependencies.Sync(dependency);

		/// <summary>
		/// 	请求组中成员响应以证实存在性
		/// 	要使用此功能必须依赖组播发送
		/// </summary>
		public void Yell() => _broadcaster.StrictField.Broadcast((byte) UdpCmd.YellAsk);

		public IReadOnlyCollection<byte> Interest => new byte[0];

		public void Process(RemotePacket remotePacket) {
			var (name, cmd, _) = remotePacket;

			if (!string.IsNullOrWhiteSpace(name)) // 非匿名则保存名字
				if (!_group.StrictField
				           .Update(name, DateTime.Now, out var previous)
				 || DateTime.Now - previous > _timeout)
					_detected?.Invoke(name);

			if (cmd == (byte) UdpCmd.YellAsk) // 回应询问
				_broadcaster.Field?.Broadcast((byte) UdpCmd.YellAck);
		}
	}
}