using MechDancer.Framework.Dependency;

namespace MechDancer.Framework.Net.Resources {
	public sealed class Name : IComponent {
		private static readonly int    Hash = typeof(Name).GetHashCode();
		public readonly         string Field;

		public Name(string name) {
			Field = name;
		}

		public override bool Equals(object obj) {
			return obj is Name;
		}

		public override int GetHashCode() {
			return Hash;
		}
	}
}