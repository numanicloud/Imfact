using Imfact.Entities;

namespace Imfact.Main;

internal class GenerationContext
{
	public Dictionary<TypeId, ConstructorRecord> Constructors { get; } = new();
    public required Logger Logger { get; init; }
}