using System;
using MechDancer.Framework.Net.Dependency;

namespace MechDancer.Framework.Net.Remote.Resources {
	public sealed class Name : IResourceFactory<object, string> {
		private readonly string _name;

		public Name(string name) => _name = name;

		public bool TryGet(object parameter, out string resource) {
			resource = _name;
			return true;
		}

		public string Get(object parameter) => _name;
		
		public override bool Equals(object obj) => obj is Name;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(Name).GetHashCode();
	}
}