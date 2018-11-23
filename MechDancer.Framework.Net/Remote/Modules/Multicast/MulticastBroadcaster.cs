using MechDancer.Framework.Net.Dependency;
using static MechDancer.Framework.Net.Dependency.Functions;
using MechDancer.Framework.Net.Remote.Resources;

namespace MechDancer.Framework.Net.Remote.Modules.Multicast {
	public class MulticastBroadcaster : AbstractModule {
		private readonly MaybeProperty<Name> _name;

//		private string Name =>
//			_name.Get(out var it) ? it.Get(null) : "";

		public MulticastBroadcaster() {
			_name = Maybe<Name>(Host);
		}
	}
}