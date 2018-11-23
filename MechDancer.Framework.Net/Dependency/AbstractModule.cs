using System;

namespace MechDancer.Framework.Net.Dependency {
	/// <summary>
	/// 	抽象功能模块
	/// </summary>
	public abstract class AbstractModule : IFunctionModule {
		private DynamicScope _host;

		protected readonly Func<DynamicScope> Host;

		protected AbstractModule() {
			Host = () => _host;
		}

		public void OnSetup(DynamicScope host) {
			_host = host;
			Sync();
		}

		public void Sync() { }

		public override bool Equals(object other) => ReferenceEquals(this, other);
		public override int  GetHashCode()        => GetType().GetHashCode();
	}
}