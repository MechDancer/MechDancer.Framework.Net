using System.Collections.Generic;

namespace MechDancer.Framework.Dependency {
	/// <summary>
	///     抽象依赖
	/// </summary>
	/// <remarks>
	///     封装了依赖项管理功能
	/// </remarks>
	public abstract class AbstractDependent<T>
		: AbstractComponent<T>, IDependent
		where T : AbstractDependent<T> {
		/// <summary>
		///     尚未装载的依赖项集
		/// </summary>
		private readonly List<IHook<IComponent>> _dependencies = new List<IHook<IComponent>>();

		/// <summary>
		///     每一次扫描都清除成功装载的依赖项
		/// </summary>
		/// <param name="dependency">新组件</param>
		/// <returns>是否装载了全部依赖项</returns>
		public virtual bool Sync(IComponent dependency)
			=> _dependencies.Count == _dependencies.RemoveAll(hook => hook.TrySet(dependency));

		/// <summary>
		///     构造一个依赖项钩子
		/// </summary>
		/// <typeparam name="TD">依赖项类型</typeparam>
		/// <returns>钩子类型</returns>
		protected ComponentHook<TD> BuildDependency<TD>()
			where TD : class, IComponent => new ComponentHook<TD>().Also(it => _dependencies.Add(it));
	}
}