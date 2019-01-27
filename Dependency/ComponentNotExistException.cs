using System;

namespace MechDancer.Framework.Dependency {
	/// <inheritdoc />
	/// <summary>
	///     组件不存在异常
	/// </summary>
	/// <typeparam name="T">不存在的组件类型</typeparam>
	public sealed class ComponentNotExistException<T> : NullReferenceException
		where T : IComponent {
		/// <inheritdoc />
		/// <summary>
		///     构造器
		/// </summary>
		public ComponentNotExistException()
			: base($"cannot find this dependency: {typeof(T).Name}") { }
	}
}
