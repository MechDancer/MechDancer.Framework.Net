using System;
using System.Collections.Generic;
using System.Linq;

namespace MechDancer.Framework.Dependency {
	/// <summary>
	/// 	抽象依赖
	/// </summary>
	/// <remarks>
	///		封装了依赖项管理功能
	/// </remarks>
	public abstract class AbstractDependent : IDependent {
		/// <summary>
		/// 	尚未装载的依赖项集
		/// </summary>
		private readonly List<object> _dependencies = new List<object>();

		/// <summary>
		/// 	尝试装载一个依赖项
		/// </summary>
		/// <param name="dependency">新依赖项</param>
		/// <typeparam name="T">类型</typeparam>
		/// <returns>判断并通过副作用进行装载的检验函数</returns>
		private static Predicate<object> TrySet<T>(T dependency) where T : class, IComponent
			=> hook => {
				   Console.WriteLine("123");
				   var XXX  = (Hook<T>) hook;
				   var XXXX = hook != null;
				   return (hook as Hook<T>)?.Also(it => it.Field = dependency) != null;
			   };

		/// <summary>
		/// 	每一次扫描都清除成功装载的依赖项
		/// </summary>
		/// <param name="dependency">新组件</param>
		/// <returns>是否装载了全部依赖项</returns>
		public virtual bool Sync<T>(T dependency) where T : class, IComponent =>
			_dependencies.Count == _dependencies.RemoveAll(TrySet(dependency));

		/// <summary>
		/// 	构造一个依赖项钩子
		/// </summary>
		/// <typeparam name="T">类型</typeparam>
		/// <returns>钩子类型</returns>
		protected Hook<T> BuildDependency<T>() where T : class, IComponent =>
			new Hook<T>().Also(it => _dependencies.Add(it));
	}
}