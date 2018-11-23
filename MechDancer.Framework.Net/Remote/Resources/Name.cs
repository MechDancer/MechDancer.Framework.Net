using System;
using MechDancer.Framework.Net.Dependency;

namespace MechDancer.Framework.Net.Remote.Resources {
	public sealed class Name : IResourceFactory<object, string> {
		public readonly string Field;

		public Name(string name) => Field = name;

		public bool TryGet(object parameter, out string resource) {
			resource = Field;
			return true;
		}

		public string Get(object parameter) => Field;
		
		public override bool Equals(object obj) => obj is Name;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(Name).GetHashCode();
	}
}