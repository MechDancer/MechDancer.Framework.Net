using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MechDancer.ConsoleParser.Scanner {
	public static class StringScanner {
		private static void Offer<T>(this IEnumerable<IScanner<T>> receiver, T item) {
			foreach (var scanner in receiver) scanner.Add(item);
		}

		private static int ToDigit(this char receiver)
			=> char.IsDigit(receiver)
				   ? receiver - '0'
				   : char.IsLower(receiver)
					   ? receiver - 'a'
					   : char.IsUpper(receiver)
						   ? receiver - 'A'
						   : throw new InvalidDataException($"'{receiver}' is not a digit");

		private static double ToDigit(this string receiver, byte order)
			=> receiver.Aggregate((0.0, 1.0),
								  (state, it) => {
									  var (sum, step) = state;
									  // ReSharper disable once CompareOfFloatsByEqualityOperator
									  return step == 1.0
												 ? it == '.'
													   ? (sum, step / order)
													   : (sum * order + it.ToDigit(), 1.0)
												 : (sum + it.ToDigit() * step, step / order);
								  }).Item1;

		private static double ToNumber(this string receiver)
			=> receiver.Length == 1 || char.IsDigit(receiver[1]) || receiver[1] == '.'
				   ? Convert.ToDouble(receiver)
				   : receiver[1] == 'b'
					   ? receiver.Substring(2).ToDigit(2)
					   : receiver[1] == 'x'
						   ? receiver.Substring(2).ToDigit(16)
						   : throw new InvalidDataException($"\"{receiver}\" is not a number");

		private static string ToKey(this string receiver)
			=> receiver[1] == '{'
				   ? receiver.Substring(2, receiver.Length - 3).Trim()
				   : receiver.Substring(1);
	}
}