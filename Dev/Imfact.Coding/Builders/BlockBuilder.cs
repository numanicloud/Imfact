using System;

namespace Imfact.Coding.Builders
{
	internal class BlockBuilder : ICodeBuilder, IDisposable
	{
		private readonly ICodeBuilder _codeBuilder;
		private bool _isOnLineStart = true;

		public string BlockEndToken { get; init; } = "}";

		private BlockBuilder(ICodeBuilder codeBuilder)
		{
			_codeBuilder = codeBuilder;
		}

		public static BlockBuilder EnterBlock(ICodeBuilder codeBuilder, string blockStartToken = "{", string blockEndToken = "}")
		{
			var instance = new BlockBuilder(codeBuilder)
			{
				BlockEndToken = blockEndToken,
			};
			codeBuilder.AppendLine(blockStartToken);
			return instance;
		}

		public void Append(string text)
		{
			_codeBuilder.Append(_isOnLineStart ? text.Indent() : text);
			_isOnLineStart = false;
		}

		public void AppendLine(string text)
		{
			_codeBuilder.AppendLine(_isOnLineStart ? text.Indent() : text);
			_isOnLineStart = true;
		}

		public string GetText() => _codeBuilder.GetText();

		public void Dispose()
		{
			_codeBuilder.AppendLine(BlockEndToken);
		}
	}
}