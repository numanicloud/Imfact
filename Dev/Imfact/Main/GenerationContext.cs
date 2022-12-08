using System.Collections.Generic;
using Imfact.Entities;

namespace Imfact.Main;

internal class GenerationContext
{
	public Dictionary<TypeId, ConstructorRecord> Constructors { get; } = new();
}