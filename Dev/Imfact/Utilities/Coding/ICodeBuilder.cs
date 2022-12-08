namespace Imfact.Utilities.Coding;

internal interface ICodeBuilder
{
    void Append(string text);
    void AppendLine(string text = "");
    string GetText();
}