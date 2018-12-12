using System;

namespace ConsoleParser.Scanner {
	public interface IScanner<in T> {
		int  Length   { get; }
		bool Complete { get; }

		void Set(T item);
		void Reset();
	}

	public static class Scanners {
		private static bool IsD(this char c) => char.IsLetter(c) || c == '_';
		private static bool Isd(this char c) => char.IsDigit(c);

		public static IScanner<char> Build(TokenType type) {
			switch (type) {
				case TokenType.Number:
					throw new NotImplementedException();
				case TokenType.Sign:
					throw new NotImplementedException();
				case TokenType.Word:
					return new DFA<char>(new[] {
						                           new[] {2, 0},
						                           new[] {2, 2},
					                           },
					                     new[] {2},
					                     it => {
						                     if (it.IsD()) return 0;
						                     if (it.Isd()) return 1;
						                     return -1;
					                     });
				case TokenType.Note:
					return new DFA<char>(new[] {
						                           new[] {2, 0, 0},
						                           new[] {7, 3, 0},
						                           new[] {6, 4, 6},
						                           new[] {5, 4, 6},
						                           new[] {0, 0, 0},
						                           new[] {6, 4, 6},
						                           new[] {7, 7, 7},
					                           },
					                     new[] {5, 7},
					                     it => {
						                     switch (it) {
							                     case '/':
								                     return 0;
							                     case '*':
								                     return 1;
							                     default:
								                     return -1;
						                     }
					                     });
				case TokenType.Key:
					return new DFA<char>(new[] {
						                           new[] {2, 0, 0, 0, 0, 0},
						                           new[] {0, 3, 0, 4, 0, 0},
						                           new[] {0, 3, 3, 0, 0, 0},
						                           new[] {5, 5, 5, 5, 0, 5},
						                           new[] {5, 5, 5, 5, 6, 5},
						                           new[] {0, 0, 0, 0, 0, 0},
					                           },
					                     new[] {3, 6},
					                     it => {
						                     if (it.IsD()) return 1;
						                     if (it.Isd()) return 2;
						                     switch (it) {
							                     case '@':
								                     return 0;
							                     case '{':
								                     return 3;
							                     case '}':
								                     return 4;
							                     default:
								                     return 5;
						                     }
					                     });
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}
	}
}