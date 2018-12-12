using System;
using System.Collections.Generic;
using System.Linq;

namespace MechDancer.Common {
	/// <summary>
	///     通用扩展函数
	/// </summary>
	public static class Extensions {
		/// <summary>
		///     链式调用函数
		/// </summary>
		/// <param name="receiver">目标对象</param>
		/// <param name="func">函数</param>
		/// <typeparam name="TI">输入类型</typeparam>
		/// <typeparam name="TO">输出类型</typeparam>
		/// <returns>映射结果</returns>
		public static TO Let<TI, TO>(this TI receiver, Func<TI, TO> func)
			=> func(receiver);

		/// <summary>
		///     插入副作用
		/// </summary>
		/// <param name="receiver">目标对象</param>
		/// <param name="func">函数</param>
		/// <typeparam name="T">目标类型</typeparam>
		/// <returns>原对象</returns>
		public static T Also<T>(this T receiver, Action<T> func) {
			func(receiver);
			return receiver;
		}

		/// <summary>
		///     用于引用类型的选通
		/// </summary>
		/// <param name="receiver">待测目标</param>
		/// <param name="predicate">测试谓词</param>
		/// <typeparam name="T">目标类型</typeparam>
		/// <returns>目标或空</returns>
		public static T TakeIf<T>(this T receiver, Predicate<T> predicate) where T : class
			=> predicate(receiver) ? receiver : null;

		/// <summary>
		///     用于值类型的选通
		/// </summary>
		/// <param name="receiver">待测目标</param>
		/// <param name="predicate">测试谓词</param>
		/// <typeparam name="T">目标类型</typeparam>
		/// <returns>目标或空</returns>
		public static T? AcceptIf<T>(this T receiver, Predicate<T> predicate) where T : struct
			=> predicate(receiver) ? (T?) receiver : null;

		/// <summary>
		///     用于引用类型的选断
		/// </summary>
		/// <param name="receiver">待测目标</param>
		/// <param name="predicate">测试谓词</param>
		/// <typeparam name="T">目标类型</typeparam>
		/// <returns>目标或空</returns>
		public static T TakeUnless<T>(this T receiver, Predicate<T> predicate) where T : class
			=> predicate(receiver) ? null : receiver;

		/// <summary>
		///     用于值类型的选断
		/// </summary>
		/// <param name="receiver">待测目标</param>
		/// <param name="predicate">测试谓词</param>
		/// <typeparam name="T">目标类型</typeparam>
		/// <returns>目标或空</returns>
		public static T? AcceptUnless<T>(this T receiver, Predicate<T> predicate) where T : struct
			=> predicate(receiver) ? null : (T?) receiver;

		/// <summary>
		///     展开一层列表的列表
		/// </summary>
		/// <param name="receiver">目标列表</param>
		/// <typeparam name="T">展开后类型</typeparam>
		/// <returns>展开的列表</returns>
		public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> receiver)
			=> receiver.SelectMany(it => it);

		/// <summary>
		///     判断列表中无元素
		/// </summary>
		/// <param name="receiver">目标列表</param>
		/// <typeparam name="T">列表元素类型</typeparam>
		/// <returns>是/否</returns>
		public static bool None<T>(this IEnumerable<T> receiver)
			=> !receiver.Any();

		/// <summary>
		///     判断列表中无符合谓词的元素
		/// </summary>
		/// <param name="receiver">目标列表</param>
		/// <param name="predicate">测试谓词</param>
		/// <typeparam name="T">列表元素类型</typeparam>
		/// <returns>是/否</returns>
		public static bool None<T>(this IEnumerable<T> receiver, Func<T, bool> predicate)
			=> !receiver.Any(predicate);

		/// <summary>
		///     筛除符合测试谓词的元素
		/// </summary>
		/// <param name="receiver">目标列表</param>
		/// <param name="predicate">测试谓词</param>
		/// <typeparam name="T">元素类型</typeparam>
		/// <returns>结果列表</returns>
		public static IEnumerable<T> WhereNot<T>(this IEnumerable<T> receiver, Func<T, bool> predicate)
			=> receiver.Where(it => !predicate(it));

		/// <summary>
		///     筛除列表中的空元素
		/// </summary>
		/// <param name="receiver">目标列表</param>
		/// <typeparam name="T">元素类型</typeparam>
		/// <returns>结果列表</returns>
		public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> receiver) where T : class
			=> receiver.Where(it => it != null);

		/// <summary>
		///     映射并筛除空元素
		/// </summary>
		/// <param name="receiver">目标列表</param>
		/// <param name="func">映射函数</param>
		/// <typeparam name="TI">输入类型</typeparam>
		/// <typeparam name="TO">输出类型</typeparam>
		/// <returns>结果列表</returns>
		public static IEnumerable<TO> SelectNotNull<TI, TO>(this IEnumerable<TI> receiver, Func<TI, TO> func)
			where TO : class => receiver.Select(func).WhereNotNull();

		/// <summary>
		///     若bool为真则执行
		/// </summary>
		/// <param name="receiver">目标bool</param>
		/// <param name="func">副作用函数</param>
		/// <returns>原bool</returns>
		public static bool Then(this bool receiver, Action func) {
			if (receiver) func();
			return receiver;
		}

		/// <summary>
		///     若bool为假则执行
		/// </summary>
		/// <param name="receiver">目标bool</param>
		/// <param name="func">副作用函数</param>
		/// <returns>原bool</returns>
		public static bool Otherwise(this bool receiver, Action func) {
			if (!receiver) func();
			return receiver;
		}
	}
}