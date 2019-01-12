using System.Collections.Generic;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Dependency.UniqueComponent;
using MechDancer.Framework.Net.Protocol;
using MechDancer.Framework.Net.Resources;

namespace MechDancer.Framework.Net.Modules.Multicast {
	/// <summary>
	/// 	话题接收者
	/// </summary>
	public class TopicReceiver : UniqueComponent<TopicReceiver>, IMulticastListener, IDependent {
		private static readonly byte[]                        InterestSet = {(byte) UdpCmd.TopicMessage};
		private readonly        UniqueDependency<TopicBuffer> _buffer;

		private readonly UniqueDependencies _dependencies = new UniqueDependencies();

		public TopicReceiver() {
			_buffer = _dependencies.BuildDependency<TopicBuffer>();
		}

		public bool Sync(IComponent dependency) => _dependencies.Sync(dependency);

		public IReadOnlyCollection<byte> Interest => InterestSet;

		public void Process(RemotePacket remotePacket) {
			var (sender, _, payload) = remotePacket;
			_buffer.StrictField.Save(sender, payload);
		}
	}
}