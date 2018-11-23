using System;

namespace MechDancer.Framework.Net.Dependency {
	public class MaybeThere<T> where T : IDependency {
		public delegate bool GetDependency(out T result);

		private          bool          _found;
		private          T             _field;
		private readonly GetDependency _func;

		public MaybeThere(GetDependency func) {
			_func = func;
		}

		/// <summary>
		/// 	尝试获取一个依赖项
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

			// 未能找到，失败
			result = default(T);
			return false;
		}
	}
}