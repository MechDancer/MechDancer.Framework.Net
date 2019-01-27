using System.Collections.Concurrent;
using System.Collections.Generic;
using MechDancer.Common;

namespace MechDancer.Framework.Dependency {
	/// <summary>
	///     动态域
	/// </summary>
	/// <remarks>
	///     动态域是域的一种，允许向其中动态地添加新的组件
	///     组件被添加到动态域时，将执行一系列操作，以自动解算依赖关系和建立组件关联
	/// </remarks>
	public sealed class DynamicScope {
		/// <summary>
		///     组件集
		/// </summary>
		/// <remarks>
		///     用于查找特定组件类型和判定类型冲突
		///     其中的组件只增加不减少
		/// </remarks>
		private readonly ConcurrentSet<IComponent> _components
			= new ConcurrentSet<IComponent>();

		/// <summary>
		///     依赖者列表
		/// </summary>
		/// <remarks>
		///     用于在在新的依赖项到来时接收通知
		///     其中的组件一旦集齐依赖项就会离开列表，不再接收通知
		/// </remarks>
		private readonly List<IDependent> _dependents
			= new List<IDependent>();

		/// <summary>
		///     浏览所有组件
		/// </summary>
		public IEnumerable<IComponent> Components => _components.View;

		/// <summary>
		///     将一个新的组件加入到动态域
		/// </summary>
		/// <param name="component">新的组件</param>
		/// <returns>
		///     若组件被添加到域，返回 true
		///     与已有的组件发生冲突时返回 false
		/// </returns>
		public bool Setup(IComponent component)
			=> _components.TryAdd(component)
						  .Then(() => {
							   lock (_dependents) {
								   _dependents.RemoveAll(it => it.Sync(component));

								   (component as IDependent)
									 ?.TakeIf(it => Components.None(it.Sync))
									 ?.Also(_dependents.Add);
							   }
						   });

		/// <summary>
		///     并发哈希集合
		/// </summary>
		/// <typeparam name="T">元素类型</typeparam>
		private sealed class ConcurrentSet<T> where T : class {
			private readonly ConcurrentDictionary<T, byte> _core = new ConcurrentDictionary<T, byte>();

			public IEnumerable<T> View => _core.Keys;

			public bool TryAdd(T it) => _core.TryAdd(it, 0);
		}
	}
}
