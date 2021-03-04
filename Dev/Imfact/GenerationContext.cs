using System.Collections.Generic;
using Imfact.Entities;
using Imfact.Interfaces;

namespace Imfact
{
	internal class GenerationContext
	{
		public Dictionary<TypeId, ConstructorRecord> Constructors { get; } = new();
	}
}
