using System;
using System.Threading;

namespace MechDancer.Framework.Dependency {
	public class Atomic<T> where T : class {
		private readonly ReaderWriterLock _lock = new ReaderWriterLock();
		private          T                _field;

		public T Field {
			get {
				_lock.AcquireReaderLock(-1);
				var value = _field;
				_lock.ReleaseReaderLock();
				return value;
			}
		}

		public T Update(Func<T, T> func) {
			_lock.AcquireWriterLock(-1);
			var value = _field = func(_field);
			_lock.ReleaseWriterLock();
			return value;
		}
	}
}