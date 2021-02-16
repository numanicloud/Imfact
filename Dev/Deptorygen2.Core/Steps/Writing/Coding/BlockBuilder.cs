using System;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Writing
{
	internal class BlockBuilder : ICodeBuilder, IDisposable
	{
		private readonly ICodeBuilder _codeBuilder;
		private bool _isOnLineStart = true;

		private BlockBuilder(ICodeBuilder codeBuilder)
		{
			_codeBuilder = codeBuilder;
		}

		public static BlockBuilder EnterBlock(ICodeBuilder codeBuilder)
		{
			var instance = new BlockBuilder(codeBuilder);
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
			_codeBuilder.AppendLine("}");
		}
	}
}