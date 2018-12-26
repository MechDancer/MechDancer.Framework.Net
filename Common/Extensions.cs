using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MechDancer.Common {
	/// <summary>
	///     通用扩展函数
	/// </summary>
	public static partial class Extensions {
		#region 副作用和插入转换

		/// <summary>
		///     链式调用函数
		/// </summary>
		/// <param name="receiver">目标对象</param>
		/// <param name="func">函数</param>
		/// <typeparam name="TI">输入类型</typeparam>
		/// <typeparam name="TO">输出类型</typeparam>
		/// <returns>映射结果</returns>
		public static TO Let<TI, TO>(this TI receiver, Func<TI, TO> func) {
			return func(receiver);
		}

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
		public static T TakeIf<T>(this T receiver, Predicate<T> predicate) where T : class {
			return predicate(receiver) ? receiver : null;
		}

		/// <summary>
		///     用于值类型的选通
		/// </summary>
		/// <param name="receiver">待测目标</param>
		/// <param name="predicate">测试谓词</param>
		/// <typeparam name="T">目标类型</typeparam>
		/// <returns>目标或空</returns>
		public static T? AcceptIf<T>(this T receiver, Predicate<T> predicate) where T : struct {
			return predicate(receiver) ? (T?) receiver : null;
		}

		/// <summary>
		///     用于引用类型的选断
		/// </summary>
		/// <param name="receiver">待测目标</param>
		/// <param name="predicate">测试谓词</param>
		/// <typeparam name="T">目标类型</typeparam>
		/// <returns>目标或空</returns>
		public static T TakeUnless<T>(this T receiver, Predicate<T> predicate) where T : class {
			return predicate(receiver) ? null : receiver;
		}

		/// <summary>
		///     用于值类型的选断
		/// </summary>
		/// <param name="receiver">待测目标</param>
		/// <param name="predicate">测试谓词</param>
		/// <typeparam name="T">目标类型</typeparam>
		/// <returns>目标或空</returns>
		public static T? AcceptUnless<T>(this T receiver, Predicate<T> predicate) where T : struct {
			return predicate(receiver) ? null : (T?) receiver;
		}

		#endregion

		#region 列表

		/// <summary>
		///     展开一层列表的列表
		/// </summary>
		/// <param name="receiver">目标列表</param>
		/// <typeparam name="T">展开后类型</typeparam>
		/// <returns>展开的列表</returns>
		public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> receiver) {
			return receiver.SelectMany(it => it);
		}

		/// <summary>
		///     判断列表中无元素
		/// </summary>
		/// <param name="receiver">目标列表</param>
		/// <typeparam name="T">列表元素类型</typeparam>
		/// <returns>是/否</returns>
		public static bool None<T>(this IEnumerable<T> receiver) {
			return !receiver.Any();
		}

		/// <summary>
		///     判断列表中无符合谓词的元素
		/// </summary>
		/// <param name="receiver">目标列表</param>
		/// <param name="predicate">测试谓词</param>
		/// <typeparam name="T">列表元素类型</typeparam>
		/// <returns>是/否</returns>
		public static bool None<T>(this IEnumerable<T> receiver, Func<T, bool> predicate) {
			return !receiver.Any(predicate);
		}

		/// <summary>
		///     筛除符合测试谓词的元素
		/// </summary>
		/// <param name="receiver">目标列表</param>
		/// <param name="predicate">测试谓词</param>
		/// <typeparam name="T">元素类型</typeparam>
		/// <returns>结果列表</returns>
		public static IEnumerable<T> WhereNot<T>(this IEnumerable<T> receiver, Func<T, bool> predicate) {
			return receiver.Where(it => !predicate(it));
		}

		/// <summary>
		///     筛除列表中的空元素
		/// </summary>
		/// <param name="receiver">目标列表</param>
		/// <typeparam name="T">元素类型</typeparam>
		/// <returns>结果列表</returns>
		public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> receiver) where T : class {
			return receiver.Where(it => it != null);
		}

		/// <summary>
		///     映射并筛除空元素
		/// </summary>
		/// <param name="receiver">目标列表</param>
		/// <param name="func">映射函数</param>
		/// <typeparam name="TI">输入类型</typeparam>
		/// <typeparam name="TO">输出类型</typeparam>
		/// <returns>结果列表</returns>
		public static IEnumerable<TO> SelectNotNull<TI, TO>(this IEnumerable<TI> receiver, Func<TI, TO> func)
			where TO : class {
			return receiver.Select(func).WhereNotNull();
		}

		#endregion

		#region bool

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

		#endregion

		#region 读写锁

		/// <summary>
		///     锁定读锁并调用方法
		/// </summary>
		/// <returns>
		///     读取到的值
		/// </returns>
		public static T Read<T>(this ReaderWriterLock receiver, Func<T> block) {
			receiver.AcquireReaderLock(-1);
			var result = block();
			receiver.ReleaseReaderLock();
			return result;
		}

		/// <summary>
		///     锁定写锁并调用方法
		/// </summary>
		public static void Write(this ReaderWriterLock receiver, Action block) {
			receiver.AcquireWriterLock(-1);
			block();
			receiver.ReleaseWriterLock();
		}

		/// <summary>
		///     锁定写锁并调用方法
		/// </summary>
		/// <returns>
		///     写入的返回值
		/// </returns>
		public static T Write<T>(this ReaderWriterLock receiver, Func<T> block) {
			receiver.AcquireWriterLock(-1);
			var result = block();
			receiver.ReleaseWriterLock();
			return result;
		}

		#endregion
	}
}