using System.Collections.Generic;

namespace MechDancer.Framework.Net.Dependency {
	/// <summary>
	/// 	功能模块
	/// </summary>
	public interface IFunctionModule : IDependency {
		/// <summary>
		/// 	加入动态域
		/// </summary>
		/// <param name="dependencies">动态域依赖项集</param>
		void OnSetup(IReadOnlyCollection<IDependency> dependencies);

		/// <summary>
		/// 	重新同步依赖项
		/// </summary>
		void Sync();
	}
}