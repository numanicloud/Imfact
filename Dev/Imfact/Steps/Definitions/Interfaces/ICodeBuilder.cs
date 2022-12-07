namespace Imfact.Steps.Definitions.Interfaces;

internal interface ICodeBuilder
{
	void Append(string text);
	void AppendLine(string text = "");
	string GetText();
}