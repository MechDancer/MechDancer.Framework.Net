using System;
using System.Collections.Generic;

namespace MechDancer.Framework.Net.Dependency {
	/// <summary>
	/// 	抽象功能模块
	/// </summary>
	public abstract class AbstractModule : IFunctionModule {
		private IReadOnlyCollection<IDependency> _dependencies;

		protected readonly Lazy<IReadOnlyCollection<IDependency>> Dependencies;

		protected AbstractModule() =>
			Dependencies = new Lazy<IReadOnlyCollection<IDependency>>
				(() => _dependencies ?? throw new InvalidOperationException(LazyMessage));

		public void OnSetup(IReadOnlyCollection<IDependency> host) {
			_dependencies = host;
			Sync();
		}

		public virtual void Sync() { }

		public override bool Equals(object other) => ReferenceEquals(this, other);
		public override int  GetHashCode()        => GetType().GetHashCode();

		private const string LazyMessage = "you must setup the module before using it";
	}
}