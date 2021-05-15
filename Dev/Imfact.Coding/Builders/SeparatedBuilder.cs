namespace Imfact.Coding.Builders
{
	internal class SeparatedBuilder : ICodeBuilder
	{
		private readonly ICodeBuilder _baseBuilder;
		private bool _isBegin = true;

		public string Separator { get; init; } = ", ";
		public string LineSeparator { get; set; } = ",";

		public SeparatedBuilder(ICodeBuilder baseBuilder)
		{
			_baseBuilder = baseBuilder;
		}

		public void Append(string text)
		{
			var comma = _isBegin ? "" : Separator;
			_baseBuilder.Append(comma + text);

			_isBegin = false;
		}

		public void AppendLine(string text = "")
		{
			if (!_isBegin)
			{
				_baseBuilder.AppendLine(LineSeparator);
			}

			_baseBuilder.Append(text);

			_isBegin = false;
		}

		public string GetText() => _baseBuilder.GetText();
	}
}
