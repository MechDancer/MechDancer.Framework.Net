namespace MechDancer.Framework.Dependency {
	/// <summary>
	/// 	组件
	/// </summary>
	/// <remarks>
	/// 	通过类型反射、哈希值和判等条件与其他组件区分开
	/// 	因此组件可以安全快捷地保存到一个哈希集合中
	/// </remarks>
	public interface IComponent { }
}