using MechDancer.Framework.Dependency;

namespace MechDancer.Framework.Net.Resources {
	public sealed class Name : AbstractComponent<Name> {
		public readonly string Field;

		public Name(string name) {
			Field = name.Trim();
		}
	}
}