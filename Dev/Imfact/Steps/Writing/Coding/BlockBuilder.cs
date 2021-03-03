using System;
using Imfact.Utilities;

namespace Imfact.Steps.Writing.Coding
{
	internal class BlockBuilder : ICodeBuilder, IDisposable
	{
		private readonly ICodeBuilder _codeBuilder;
		private bool _isOnLineStart = true;

		public bool WithTrailingSemicolon { get; init; } = false;

		private BlockBuilder(ICodeBuilder codeBuilder)
		{
			_codeBuilder = codeBuilder;
		}

		public static BlockBuilder EnterBlock(ICodeBuilder codeBuilder, bool withTrailingSemicolon = false)
		{
			var instance = new BlockBuilder(codeBuilder)
			{
				WithTrailingSemicolon = withTrailingSemicolon
			};
			codeBuilder.AppendLine("{");
			return instance;
		}

		public void Append(string text)
		{
			_codeBuilder.Append(_isOnLineStart ? text.Indent(1) : text);
			_isOnLineStart = false;
		}

		public void AppendLine(string text)
		{
			_codeBuilder.AppendLine(_isOnLineStart ? text.Indent(1) : text);
			_isOnLineStart = true;
		}

		public string GetText() => _codeBuilder.GetText();

		public void Dispose()
		{
			var semicolon = WithTrailingSemicolon ? ";" : "";
			_codeBuilder.AppendLine("}" + semicolon);
		}
	}
}