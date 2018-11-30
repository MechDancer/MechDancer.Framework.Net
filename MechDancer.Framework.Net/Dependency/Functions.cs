using System;
using System.Collections.Generic;
using System.Linq;

namespace MechDancer.Framework.Net.Dependency {
	public static class Functions {
		/// <summary>
		/// 	从动态域找到目标类型的依赖项
		/// </summary>
		/// <param name="receiver">目标动态域</param>
		/// <typeparam name="T">目标类型</typeparam>
		/// <returns>依赖项集</returns>
		public static IEnumerable<T> Get<T>(this IReadOnlyCollection<IDependency> receiver)
			where T : class, IDependency =>
			receiver.Select(it => it as T)
			        .Where(it => it != null);

		/// <summary>
		/// 	尝试从动态域找到目标类型的唯一依赖项
		/// </summary>
		/// <param name="receiver">目标动态域</param>
		/// <typeparam name="T">目标类型</typeparam>
		/// <returns>唯一依赖项或空</returns>
		public static T Maybe<T>(this IReadOnlyCollection<IDependency> receiver)
			where T : class, IDependency =>
			receiver.Get<T>().SingleOrDefault();

		/// <summary>
		/// 	从动态域找到目标类型的唯一依赖项
		/// </summary>
		/// <param name="receiver">目标动态域</param>
		/// <typeparam name="T">目标类型</typeparam>
		/// <returns>唯一依赖项</returns>
		public static T Must<T>(this IReadOnlyCollection<IDependency> receiver)
			where T : class, IDependency =>
			receiver.Maybe<T>() ?? throw new Exception($"cannot find this dependency: {typeof(T).Name}");

		/// <summary>
		/// 	构造动态域，操作，扫描，并返回
		/// </summary>
		/// <param name="block">操作</param>
		/// <returns>新的动态域</returns>
		public static DynamicScope Scope(Action<DynamicScope> block) =>
			new DynamicScope().Also(it => {
				                        block(it);
				                        it.Sync();
			                        });
	}
}