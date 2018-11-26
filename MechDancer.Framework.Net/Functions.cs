using System;

namespace MechDancer.Framework.Net {
	public static class Functions {
		public static U Let<T, U>(this T receiver, Func<T, U> block) =>
			block(receiver);

		public static T Also<T>(this T receiver, Action<T> block) {
			block(receiver);
			return receiver;
		}
	}
}