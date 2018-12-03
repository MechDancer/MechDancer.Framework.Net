namespace MechDancer.Framework.Dependency {
	/// <summary>
	/// 	引用钩子类型
	/// </summary>
	/// <typeparam name="T">内部类型</typeparam>
	public sealed class Hook<T> where T : class, IComponent {
		/// <exception cref="System.NullReferenceException"></exception>
		public T Field;

		public T StrictField => Field ?? throw new ComponentNotExistException(typeof(T));
	}
}