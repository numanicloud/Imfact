using System;
using System.Text;

namespace Imfact.Steps.Writing.Coding
{
	internal class ChunkBuilder : ICodeBuilder, IDisposable
	{
		private readonly ICodeBuilder _baseBuilder;
		private readonly StringBuilder _stringBuilder = new ();

		public ChunkBuilder(ICodeBuilder baseBuilder)
		{
			_baseBuilder = baseBuilder;
		}

		public void Append(string text) => _stringBuilder.Append(text);

		public void AppendLine(string text) => _stringBuilder.AppendLine(text);

		public string GetText() => _stringBuilder.ToString();

		public void Dispose()
		{
			var text = _stringBuilder.ToString().TrimEnd();
			if (text != string.Empty)
			{
				_baseBuilder.AppendLine(text);
			}
		}
	}
}