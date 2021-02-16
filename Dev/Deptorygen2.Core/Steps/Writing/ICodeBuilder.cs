namespace Deptorygen2.Core.Steps.Writing
{
	internal interface ICodeBuilder
	{
		void Append(string text);
		void AppendLine(string text = "");
		string GetText();
	}
}