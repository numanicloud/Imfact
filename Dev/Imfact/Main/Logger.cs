namespace Imfact.Main;

internal class Logger
{
    private readonly Action<string> _logDebug;

    public Logger(Action<string> logDebug)
	{
        this._logDebug = logDebug;
    }

    public void Debug(string message)
    {
        _logDebug(message);
    }
}