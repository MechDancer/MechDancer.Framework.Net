using System;

namespace MechDancer.ConsoleParser.Scanner {
	/// <summary>
	///     扫描器接口
	/// </summary>
	/// <typeparam name="T">单体类型</typeparam>
	public interface IScanner<in T> {
		/// <summary>
		///     最长匹配长度
		/// </summary>
		int Length { get; }

		/// <summary>
		///     匹配是否完整
		/// </summary>
		bool Complete { get; }

		/// <summary>
		///     匹配一个单体
		/// </summary>
		/// <param name="item"></param>
		void Add(T item);

		/// <summary>
		///     重置匹配状态
		/// </summary>
		void Reset();
	}

	public static class Scanners {
		private static bool IsD(this char c) => char.IsLetter(c) || c == '_';

		private static bool Isd(this char c) => char.IsDigit(c);

		public static IScanner<char> Build(TokenType type) {
			switch (type) {
				case TokenType.Sign:
					return new SignScanner();
				case TokenType.Word:
					return new DFA<char>(new[] {
						                           new[] {2, 0},
						                           new[] {2, 2}
					                           },
					                     new[] {2},
					                     it => {
						                     if (it.IsD()) return 0;
						                     if (it.Isd()) return 1;
						                     return -1;
					                     });
				case TokenType.Note:
					return new DFA<char>(new[] {
						                           new[] {2, 0, 0}, // 1 -> ε
						                           new[] {7, 3, 0}, // 2 -> /
						                           new[] {6, 4, 6}, // 3 -> /*
						                           new[] {5, 4, 6}, // 4 -> /* ... *
						                           new[] {0, 0, 0}, // 5 -> /* ... */
						                           new[] {6, 4, 6}, // 6 -> /* ...
						                           new[] {7, 7, 7}  // 7 -> // ...
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
						                           new[] {2, 0, 0, 0, 0, 0}, // 1 -> ε
						                           new[] {0, 3, 0, 4, 0, 0}, // 2 -> @
						                           new[] {0, 3, 3, 0, 0, 0}, // 3 -> @ ...
						                           new[] {5, 5, 5, 5, 0, 5}, // 4 -> @{
						                           new[] {5, 5, 5, 5, 6, 5}, // 5 -> @{ ...
						                           new[] {0, 0, 0, 0, 0, 0}  // 6 -> @{ ... }
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
				case TokenType.Number:
					return new DFA<char>(new[] {
						                           new[] {+2, 11, 11, +0, +0, 0, 12}, // 1  -> ε
						                           new[] {11, 11, 11, +3, +0, 7, 12}, // 2  -> 0
						                           new[] {+4, +4, +0, +0, +0, 0, +5}, // 3  -> 0b
						                           new[] {+4, +4, +0, +0, +0, 0, +5}, // 4  -> 0b...
						                           new[] {+6, +6, +0, +0, +0, 0, +0}, // 5  -> 0b... .
						                           new[] {+6, +6, +0, +0, +0, 0, +0}, // 6  -> 0b... . ...
						                           new[] {+8, +8, +8, +8, +8, 0, +9}, // 7  -> 0x
						                           new[] {+8, +8, +8, +8, +8, 0, +9}, // 8  -> 0x...
						                           new[] {10, 10, 10, 10, 10, 0, +0}, // 9  -> 0x... .
						                           new[] {10, 10, 10, 10, 10, 0, +0}, // 10 -> 0x... . ...
						                           new[] {11, 11, 11, +0, +0, 0, 12}, // 11 -> ...
						                           new[] {13, 13, 13, +0, +0, 0, +0}, // 12 -> ... .
						                           new[] {13, 13, 13, +0, +0, 0, +0}  // 13 -> ... . ...
					                           },
					                     new[] {2, 4, 6, 8, 10, 11, 13},
					                     it => {
						                     switch (char.ToLowerInvariant(it)) {
							                     case '0':
								                     return 0;
							                     case '1':
								                     return 1;
							                     case '2':
							                     case '3':
							                     case '4':
							                     case '5':
							                     case '6':
							                     case '7':
							                     case '8':
							                     case '9':
								                     return 2;
							                     case 'b':
								                     return 3;
							                     case 'a':
							                     case 'c':
							                     case 'd':
							                     case 'e':
							                     case 'f':
								                     return 4;
							                     case 'x':
								                     return 5;
							                     case '.':
								                     return 6;
							                     default:
								                     return -1;
						                     }
					                     });
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}
	}
}