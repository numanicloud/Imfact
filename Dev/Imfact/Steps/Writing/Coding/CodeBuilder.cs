using System.Text;
using Imfact.Steps.Definitions.Interfaces;

namespace Imfact.Steps.Writing.Coding
{
	internal class CodeBuilder : ICodeBuilder
	{
		private readonly StringBuilder _builder = new();

		public CodeBuilder(StringBuilder? builder = null)
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