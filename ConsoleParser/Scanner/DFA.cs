using System;
using System.Collections.Generic;
using System.Linq;
using MechDancer.Common;

// ReSharper disable InconsistentNaming

namespace MechDancer.ConsoleParser.Scanner {
	/// <inheritdoc />
	/// <summary>
	///     确定性自动机扫描器
	/// </summary>
	/// <typeparam name="T">单体类型</typeparam>
	public class DFA<T> : IScanner<T> where T : struct {
		private readonly IEnumerable<int> _ending;
		private readonly Func<T, int>     _map;
		private readonly int[][]          _table;

		/// <summary>
		///     当前状态
		/// </summary>
		/// <remarks>
		///     正数表示正在匹配
		///     负数是匹配失败前最后状态的相反数，即能匹配部分的结束状态
		/// </remarks>
		private int _state = 1;

		public DFA(int[][] table, IEnumerable<int> ending, Func<T, int> map) {
			_table  = table;
			_ending = ending;
			_map    = map;
		}

		public int  Length   { get; private set; }
		public bool Complete => _ending.Contains(Math.Abs(_state));

		public void Add(T item) {
			if (_state > 0)
				_state = _map(item)
				        .AcceptIf(it => it >= 0)
				       ?.Let(it => _table[_state - 1][it])
				        .AcceptIf(it => it != 0)
				       ?.Also(_ => ++Length)
				      ?? -_state;
		}

		public void Reset() {
			_state = 1;
			Length = 0;
		}
	}
}