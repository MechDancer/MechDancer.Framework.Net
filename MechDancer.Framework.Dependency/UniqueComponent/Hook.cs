using System.Threading;

namespace MechDancer.Framework.Dependency.UniqueComponent {
	/// <summary>
	///     用于标定引用钩子转型上限的共通基类型
	/// </summary>
	/// <typeparam name="T">钩子类型转型上限</typeparam>
	public interface IHook<in T> {
		bool TrySet(T obj);
	}

	/// <summary>
	///     引用钩子类型
	/// </summary>
	/// <typeparam name="T">内部类型</typeparam>
	/// <typeparam name="TS">设值上限</typeparam>
	public class Hook<T, TS> : IHook<TS> where T : class, TS {
		private readonly ReaderWriterLock _lock = new ReaderWriterLock();
		private          T                _field;

		public T Field {
			get {
				_lock.AcquireReaderLock(-1);
				var value = _field;
				_lock.ReleaseReaderLock();
				return value;
			}
			set {
				_lock.AcquireWriterLock(-1);
				_field = value;
				_lock.ReleaseWriterLock();
			}
		}

		public bool TrySet(TS obj) {
			return (obj as T)?.Also(it => Field = it) != null;
		}
	}

	/// <summary>
	///     组件的引用钩子
	/// </summary>
	/// <typeparam name="T">组件类型</typeparam>
	public class UniqueDependency<T> : Hook<T, IComponent> where T : class, IComponent {
		public T StrictField => Field ?? throw new ComponentNotExistException(typeof(T));
	}
}