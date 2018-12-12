using System;

namespace MechDancer.ConsoleParser {
	/// <summary>
	///     词性
	/// </summary>
	public enum TokenType : byte {
		Number,
		Sign,
		Word,
		Note,
		Key
	}

	/// <summary>
	///     公共基类
	/// </summary>
	public interface IToken {
		TokenType TokenType { get; }
	}

	/// <summary>
	///     单词
	/// </summary>
	/// <typeparam name="T">类型</typeparam>
	public sealed class Token<T> : IToken {
		public Token(TokenType type, T data) {
			TokenType = type;
			Data      = data;
		}

		public T         Data      { get; }
		public TokenType TokenType { get; }

		/// <summary>
		///     判断两个词是否匹配
		/// </summary>
		public bool Match(IToken other) {
			if (TokenType != other.TokenType
			 && TokenType != TokenType.Key) return false;

			switch (TokenType) {
				case TokenType.Sign:
				case TokenType.Word:
					return (this as Token<string>)?.Data
					    == (other as Token<string>)?.Data;
				case TokenType.Number:
				case TokenType.Note:
				case TokenType.Key:
					return true;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public override string ToString() => Data as string ?? Data.ToString();
	}
}