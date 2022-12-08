using System.Collections.Generic;
using Imfact.Entities;

namespace Imfact.Main;

internal class GenerationContext
{
	public Dictionary<TypeId, ConstructorRecord> Constructors { get; } = new();
    public required Logger Logger { get; init; }
}

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