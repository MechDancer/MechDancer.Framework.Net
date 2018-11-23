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
		public static IEnumerable<T> Get<T>(this DynamicScope receiver)
			where T : class, IDependency =>
			receiver.Dependencies
			        .Select(it => it as T)
			        .Where(it => it != null);

		/// <summary>
		/// 	尝试从动态域找到目标类型的唯一依赖项
		/// </summary>
		/// <param name="receiver">目标动态域</param>
		/// <typeparam name="T">目标类型</typeparam>
		/// <returns>唯一依赖项或空</returns>
		public static T Maybe<T>(this DynamicScope receiver)
			where T : class, IDependency =>
			receiver.Get<T>().SingleOrDefault();

		/// <summary>
		/// 	从动态域找到目标类型的唯一依赖项
		/// </summary>
		/// <param name="receiver">目标动态域</param>
		/// <typeparam name="T">目标类型</typeparam>
		/// <returns>唯一依赖项</returns>
		public static T Must<T>(this DynamicScope receiver)
			where T : class, IDependency =>
			receiver.Maybe<T>() ?? throw new Exception($"cannot find this dependency: {typeof(T).Name}");

		/// <summary>
		/// 	代理属性构建器
		/// </summary>
		private class DelegateBuilder {
			private readonly Func<DynamicScope> _host;

			public DelegateBuilder(Func<DynamicScope> host) =>
				_host = host;

			public bool MaybeDelegate<T>(out T result)
				where T : class, IDependency =>
				(result = _host().Maybe<T>()) != null;
		}

		/// <summary>
		/// 	构造一个每次检查依赖项的代理属性
		/// </summary>
		/// <param name="func">惰性获取所在动态域的方法</param>
		/// <typeparam name="T">目标依赖项类型</typeparam>
		/// <returns>代理属性</returns>
		public static MaybeProperty<T> Maybe<T>(Func<DynamicScope> func)
			where T : class, IDependency =>
			new MaybeProperty<T>(new DelegateBuilder(func).MaybeDelegate);

		/// <summary>
		/// 	构造一个严格要求依赖项的代理属性
		/// </summary>
		/// <param name="func">惰性获取所在动态域的方法</param>
		/// <typeparam name="T">目标依赖项类型</typeparam>
		/// <returns>代理属性</returns>
		public static MustProperty<T> Must<T>(Func<DynamicScope> func)
			where T : class, IDependency =>
			new MustProperty<T>(() => func().Must<T>());

		/// <summary>
		/// 	构造动态域，操作，扫描，并返回
		/// </summary>
		/// <param name="block">操作</param>
		/// <returns>新的动态域</returns>
		public static DynamicScope Scope(Action<DynamicScope> block) {
			var result = new DynamicScope();
			block(result);
			result.Sync();
			return result;
		}
	}
}