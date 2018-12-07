using MechDancer.Framework.Dependency;
using MechDancer.Framework.Dependency.UniqueComponent;

namespace MechDancer.Framework.Net.Resources {
	public sealed class Name : UniqueComponent<Name> {
		public readonly string Field;

		public Name(string name) {
			Field = name.Trim();
		}
	}
}