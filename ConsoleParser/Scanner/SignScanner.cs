using System;
using System.Collections.Generic;
using System.Linq;
using MechDancer.Common;

namespace MechDancer.ConsoleParser.Scanner {
	/// <summary>
	///     符号扫描器
	/// </summary>
	/// <remarks>
	///     传统算法实现
	/// </remarks>
	public class SignScanner : IScanner<char> {
		private static readonly char[] Signs
			= {
				  '!', '?', '@', '#', '$',
				  '+', '-', '*', '/', '%', '^',
				  '&', '|', '~', '_', '=',
				  ':', '<', '>', '.',

				  '`', '\'', '\"',
				  '(', ')', '[', ']', '{', '}',
				  ';', ',', '\n'
			  };

		private static readonly string[] DefaultExtensions
			= {
				  "++", "--", "**", "&&", "||",
				  "<<", ">>", "->", "<-",
				  "==", "!=", "===", "=/=",
				  "=>", "<=>", "<=", ">=",
				  "..", "...", "?.", "??", "!!",
				  "<>", ":=", "@@", "/@", "/;"
			  };

		private readonly char[]              _buffer;
		private readonly IEnumerable<string> _extensions;

		private int _state;

		/// <summary>
		///     构造器
		/// </summary>
		/// <param name="extensions">扩充符号集</param>
		public SignScanner(IEnumerable<string> extensions = null) {
			_extensions = extensions ?? DefaultExtensions;
			_buffer     = new char[Math.Max(1, _extensions.Select(it => it.Length).Max())];
		}

		public int  Length   => _state.AcceptIf(it => it >= 0) ?? -(_state + 1);
		public bool Complete => Length > 0;

		public void Add(char item) {
			if (_state < 0) return;
			if (!Signs.Contains(item) || _state == _buffer.Length) {
				_state = -_state - 1;
				return;
			}

			_buffer[_state] = item;
			var current = new string(_buffer.CopyRange(0, ++_state));
			if (_state > 1 && _extensions.None(it => it.StartsWith(current)))
				_state = -_state;
		}

		public void Reset() {
			_state = 0;
		}
	}
}