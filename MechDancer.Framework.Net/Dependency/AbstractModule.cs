using System;

namespace MechDancer.Framework.Net.Dependency {
	/// <summary>
	/// 	抽象功能模块
	/// </summary>
	public abstract class AbstractModule : IFunctionModule {
		private DynamicScope _host;

		protected readonly Lazy<DynamicScope> Host;

		protected AbstractModule() =>
			Host = new Lazy<DynamicScope>
				(() => _host ?? throw new InvalidOperationException(LazyMessage));

		public void OnSetup(DynamicScope host) {
			_host = host;
			Sync();
		}

		public virtual void Sync() { }

		public override bool Equals(object other) => ReferenceEquals(this, other);
		public override int  GetHashCode()        => GetType().GetHashCode();

		private const string LazyMessage = "you must setup the module before using it";
	}
}