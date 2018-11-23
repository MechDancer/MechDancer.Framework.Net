using System;

namespace MechDancer.Framework.Net.Dependency {
	/// <summary>
	/// 	在使用时获取目标的代理属性
	/// </summary>
	/// <typeparam name="T">目标类型</typeparam>
	public class MustProperty<T> {
		private readonly Func<T> _func;
		private          bool    _found;
		private          T       _field;

		/// <summary>
		/// 	构造器
		/// </summary>
		/// <param name="func">查找方法</param>
		public MustProperty(Func<T> func) {
			_func = func;
		}

		/// <summary>
		/// 	获取目标
		/// </summary>
		public T Field {
			get {
				// 已经找到，直接返回缓存
				if (_found) return _field;

				// 此次找到，缓存并返回
				_found = true;
				return _field = _func();
			}
		}
	}
}