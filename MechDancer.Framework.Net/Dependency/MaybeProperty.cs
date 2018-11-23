using System;

namespace MechDancer.Framework.Net.Dependency {
	/// <summary>
	/// 	在每次需要时都尝试获取目标的代理属性
	/// </summary>
	/// <typeparam name="T">目标类型</typeparam>
	public class MaybeProperty<T> {
		/// <summary>
		/// 	尝试获取使用的函数类型
		/// </summary>
		/// <param name="result">结果</param>
		/// <returns>是否获取成功</returns>
		public delegate bool GetDependency(out T result);

		private readonly GetDependency _func;
		private          bool          _found;
		private          T             _field;

		/// <summary>
		/// 	构造器
		/// </summary>
		/// <param name="func">查找方法</param>
		public MaybeProperty(GetDependency func) {
			_func = func;
		}

		/// <summary>
		/// 	尝试获取目标
		/// </summary>
		/// <param name="result">结果</param>
		/// <returns>是否获取成功</returns>
		bool Get(out T result) {
			// 已经找到，直接返回缓存
			if (_found) {
				result = _field;
				return true;
			}

			// 此次找到，缓存并返回
			if (_func(out _field)) {
				result = _field;
				return _found = true;
			}

			// 未能找到，告知失败
			result = default(T);
			return false;
		}
	}
}