using System.Text;

namespace Imfact.Coding.Builders
{
	internal class PlainBuilder : ICodeBuilder
	{
		private readonly StringBuilder _builder = new();

		public PlainBuilder(StringBuilder? builder = null)
		{
			if (builder is not null)
			{
				_builder = builder;
			}
		}

		public void Append(string text)
		{
			_builder.Append(text);
		}

		public void AppendLine(string text)
		{
			_builder.AppendLine(text);
		}

		public string GetText()
		{
			return _builder.ToString();
		}
	}
}