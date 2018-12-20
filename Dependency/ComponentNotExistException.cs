using System;

namespace MechDancer.Framework.Dependency {
	/// <summary>
	///     组件不存在异常
	/// </summary>
	/// <param name="T">不存在的组件类型</param>
	public sealed class ComponentNotExistException<T> : NullReferenceException
		where T : IComponent {
		/// <summary>
		///     构造器
		/// </summary>
		public ComponentNotExistException()
			: base($"cannot find this dependency: {typeof(T).Name}") { }
	}
}