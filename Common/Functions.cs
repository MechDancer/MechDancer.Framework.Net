using System;
using System.Collections.Generic;
using System.Linq;

namespace MechDancer.Common {
	public static class Extensions {
		public static U Let<T, U>(this T receiver, Func<T, U> block) => block(receiver);

		public static T Also<T>(this T receiver, Action<T> block) {
			block(receiver);
			return receiver;
		}

		public static T TakeIf<T>(this T receiver, Predicate<T> predicate)
			where T : class => predicate(receiver) ? receiver : null;

		public static T? AcceptIf<T>(this T receiver, Predicate<T> predicate)
			where T : struct => predicate(receiver) ? (T?) receiver : null;

		public static T TakeUnless<T>(this T receiver, Predicate<T> predicate)
			where T : class => predicate(receiver) ? null : receiver;

		public static T? AcceptUnless<T>(this T receiver, Predicate<T> predicate)
			where T : struct => predicate(receiver) ? null : (T?) receiver;

		public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> receiver)
			=> receiver.SelectMany(it => it);

		public static bool None<T>(this IEnumerable<T> receiver)
			=> !receiver.Any();

		public static bool None<T>(this IEnumerable<T> receiver, Func<T, bool> predicate)
			=> !receiver.Any(predicate);

		public static IEnumerable<T> WhereNot<T>(this IEnumerable<T> receiver, Func<T, bool> predicate)
			=> receiver.Where(it => !predicate(it));

		public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> receiver)
			where T : class => receiver.Where(it => it != null);

		public static IEnumerable<TO> SelectNotNull<TI, TO>(this IEnumerable<TI> receiver, Func<TI, TO> func)
			where TO : class => receiver.Select(func).WhereNotNull();

		public static bool Then(this bool receiver, Action func) {
			if (receiver) func();
			return receiver;
		}

		public static bool Otherwise(this bool receiver, Action func) {
			if (!receiver) func();
			return receiver;
		}
	}
}