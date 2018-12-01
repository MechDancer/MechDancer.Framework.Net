using System;

namespace MechDancer.Framework.Dependency {
	public static class Extensions {
		public static U Let<T, U>(this T receiver, Func<T, U> block)
			=> block(receiver);

		public static T Also<T>(this T receiver, Action<T> block) {
			block(receiver);
			return receiver;
		}

		public static T TakeIf<T>(this T receiver, Predicate<T> predicate) where T : class
			=> predicate(receiver) ? receiver : null;
	}
}