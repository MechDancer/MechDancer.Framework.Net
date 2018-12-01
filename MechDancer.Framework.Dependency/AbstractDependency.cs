namespace MechDancer.Framework.Dependency {
	public class Dependency<T> where T : class, IComponent {
		private readonly Atomic<T> _field = new Atomic<T>();

		public T Set(IComponent value) => _field.Update(it => value as T ?? it);

		public virtual T Field => _field.Field;
	}

	public class StrictDependency<T> : Dependency<T> where T : class, IComponent {
		public override T Field => base.Field ?? throw new ComponentNotExistException(typeof(T));
	}
}