namespace MechDancer.Framework.Dependency.UniqueComponent {
	/// <summary>
	/// 	抽象组件
	/// </summary>
	/// <remarks>
	///		提供判等和哈希的默认重载
	/// </remarks>
	/// <typeparam name="T">类型</typeparam>
	public abstract class UniqueComponent<T> : IComponent where T : UniqueComponent<T> {
		private static readonly int TypeHash = typeof(T).GetHashCode();

		public override bool Equals(object obj) => ReferenceEquals(this, obj) || obj is T;
		public override int  GetHashCode()      => TypeHash;
	}
}