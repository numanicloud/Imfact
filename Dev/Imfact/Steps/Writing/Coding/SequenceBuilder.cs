using Imfact.Interfaces;
using Imfact.Steps.Writing.Coding;

namespace Imfact.Steps.Writing
{
	internal class SequenceBuilder : ICodeBuilder
	{
		private readonly ICodeBuilder _codeBuilder;
		private bool _isFirstLine = true;
		private bool _isLineStart = true;

		public SequenceBuilder(ICodeBuilder codeBuilder)
		{
			_codeBuilder = codeBuilder;
		}

		public void Append(string text)
		{
			if (!_isFirstLine && _isLineStart)
			{
				_codeBuilder.AppendLine();
			}

			_codeBuilder.Append(text);
			_isLineStart = false;
		}

		public void AppendLine(string text)
		{
			if (!_isFirstLine && _isLineStart)
			{
				_codeBuilder.AppendLine();
			}

			_codeBuilder.AppendLine(text);
			_isFirstLine = false;
			_isLineStart = true;
		}

		public string GetText() => _codeBuilder.GetText();
	}
}