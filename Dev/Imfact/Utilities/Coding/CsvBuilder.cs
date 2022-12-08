namespace Imfact.Utilities.Coding;

internal class CsvBuilder : ICodeBuilder
{
    private readonly ICodeBuilder _baseBuilder;
    private bool _isBegin = true;

    public CsvBuilder(ICodeBuilder baseBuilder)
    {
        _baseBuilder = baseBuilder;
    }

    public void Append(string text)
    {
        var comma = _isBegin ? "" : ", ";
        _baseBuilder.Append(comma + text);

        _isBegin = false;
    }

    public void AppendLine(string text = "")
    {
        if (!_isBegin)
        {
            _baseBuilder.AppendLine(",");
        }

        _baseBuilder.Append(text);

        _isBegin = false;
    }

    public string GetText() => _baseBuilder.GetText();
}