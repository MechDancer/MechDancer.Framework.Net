using System.Collections.Generic;
using System.Linq;

namespace MechDancer.Framework.Net.Dependency {
	/// <summary>
	/// 	动态域
	/// </summary>
	/// <remarks>
	///		共享的功能和相互依赖的功能的依赖范围
	/// </remarks> 
	public class DynamicScope {
		private readonly HashSet<IDependency> _dependencies = new HashSet<IDependency>();

		/// <summary>
		/// 	浏览所有依赖项
		/// </summary>
		public IReadOnlyCollection<IDependency> Dependencies => _dependencies;

		/// <summary>
		/// 	加载一个新的依赖项
		/// </summary>
		/// <param name="dependency">依赖项</param>
		/// <returns>是否加载成功</returns>
		public bool Setup(IDependency dependency) {
			var result = _dependencies.Add(dependency);
			(dependency as IFunctionModule)?.Sync();
			return result;
		}

		/// <summary>
		/// 	重新同步依赖项
		/// </summary>
		public void Sync() {
			foreach (var dependency in _dependencies.Select(it => it as IFunctionModule))
				dependency?.Sync();
		}
	}
}