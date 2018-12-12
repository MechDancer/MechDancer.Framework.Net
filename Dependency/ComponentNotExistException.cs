using System;
using System.Reflection;

namespace MechDancer.Framework.Dependency {
	/// <summary>
	///     组件不存在异常
	/// </summary>
	public sealed class ComponentNotExistException : NullReferenceException {
		/// <summary>
		///     构造器
		/// </summary>
		/// <param name="type">不存在的组件类型</param>
		public ComponentNotExistException(MemberInfo type)
			: base($"cannot find this dependency: {type.Name}") { }
	}
}