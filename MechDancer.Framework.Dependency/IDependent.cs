namespace MechDancer.Framework.Dependency {
	/// <summary>
	/// 	依赖项
	/// </summary>
	/// <remarks>
	///		依赖者的内容不完整，需要获得依赖项的支持
	///		因此当一个新的依赖项到达，依赖者会得到通知，直到所有依赖项都集齐为止
	/// </remarks>
	public interface IDependent : IComponent {
		/// <summary>
		/// 	为依赖者添加一个新的依赖项
		/// </summary>
		/// <param name="dependency">新加入域的组件</param>
		/// <returns>否已获得全部依赖项</returns>
		bool Sync(IComponent dependency);
	}
}