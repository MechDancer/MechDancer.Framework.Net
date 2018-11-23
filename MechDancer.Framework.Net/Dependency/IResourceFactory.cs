namespace MechDancer.Framework.Net.Dependency {
	/// <summary>
	/// 	资源工厂
	/// </summary>
	/// <typeparam name="TP">参数类型</typeparam>
	/// <typeparam name="TR">资源类型</typeparam>
	public interface IResourceFactory<in TP, TR> : IDependency {
		/// <summary>
		/// 	获取资源
		/// </summary>
		/// <param name="parameter">参数</param>
		/// <param name="resource">资源</param>
		/// <returns>资源是否存在</returns>
		bool Get(TP parameter, out TR resource);
	}
}