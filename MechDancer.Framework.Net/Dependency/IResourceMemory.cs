namespace MechDancer.Framework.Net.Dependency {
	/// <summary>
	/// 	资源缓存
	/// </summary>
	/// <typeparam name="TP">参数类型</typeparam>
	/// <typeparam name="TR">资源类型</typeparam>
	public interface IResourceMemory<in TP, TR>
		: IResourceFactory<TP, TR> {
		/// <summary>
		/// 	更新资源
		/// </summary>
		/// <param name="parameter">对应参数</param>
		/// <param name="resource">资源</param>
		/// <param name="previous">更新之前的资源</param>
		/// <returns>更新之前资源是否存在</returns>
		bool Update(TP parameter, TR resource, out TR previous);
	}
}