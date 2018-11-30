using System;
using System.Collections.Generic;

namespace MechDancer.Framework.Net.Dependency {
	/// <summary>
	/// 	抽象功能模块
	/// </summary>
	public abstract class AbstractModule : IFunctionModule {
		protected IReadOnlyCollection<IDependency> Dependencies;

		public void OnSetup(IReadOnlyCollection<IDependency> dependencies) {
			Dependencies = dependencies;
			Sync();
		}

		public virtual void Sync() { }
		
		/// <summary>
		/// 	构造一个每次检查依赖项的代理属性
		/// </summary>
		/// <typeparam name="T">目标依赖项类型</typeparam>
		/// <returns>代理属性</returns>
		protected Lazy<T> Maybe<T>()
			where T : class, IDependency =>
			new Lazy<T>(() => Dependencies.Maybe<T>());

		/// <summary>
		/// 	构造一个严格要求依赖项的代理属性
		/// </summary>
		/// <typeparam name="T">目标依赖项类型</typeparam>
		/// <returns>代理属性</returns>
		protected Lazy<T> Must<T>()
			where T : class, IDependency =>
			new Lazy<T>(() => Dependencies.Must<T>());

		public override bool Equals(object other) => ReferenceEquals(this, other);
		public override int  GetHashCode()        => GetType().GetHashCode();
	}
}