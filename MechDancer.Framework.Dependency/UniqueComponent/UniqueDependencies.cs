using System.Collections.Generic;

namespace MechDancer.Framework.Dependency.UniqueComponent {
	/// <summary>
	/// 	管理单例依赖项
	/// </summary>
	public sealed class UniqueDependencies {
		/// <summary>
		///     尚未装载的依赖项集
		/// </summary>
		private readonly List<IHook<IComponent>> _dependencies = new List<IHook<IComponent>>();

		/// <summary>
		///     每一次扫描都清除成功装载的依赖项
		/// </summary>
		/// <param name="dependency">新组件</param>
		/// <returns>是否装载了全部依赖项</returns>
		public bool Sync(IComponent dependency)
			=> _dependencies.Count == _dependencies.RemoveAll(hook => hook.TrySet(dependency));

		/// <summary>
		///     构造一个依赖项钩子
		/// </summary>
		/// <typeparam name="TD">依赖项类型</typeparam>
		/// <returns>钩子类型</returns>
		public UniqueDependency<TD> BuildDependency<TD>()
			where TD : class, IComponent => new UniqueDependency<TD>().Also(it => _dependencies.Add(it));
	}
}