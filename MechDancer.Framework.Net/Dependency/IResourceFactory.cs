namespace MechDancer.Framework.Net.Dependency {
	/// <summary>
	/// 	资源工厂
	/// </summary>
	/// <typeparam name="TP">参数类型</typeparam>
	/// <typeparam name="TR">资源类型</typeparam>
	public interface IResourceFactory<in TP, TR> : IDependency {
		/// <summary>
		/// 	尝试获取资源
		/// </summary>
		/// <param name="parameter">参数</param>
		/// <returns>资源</returns>
		TR Get(TP parameter);
	}
}