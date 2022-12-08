using Imfact.Entities;

namespace Imfact.Steps.Definitions;

internal class DefinitionResult
{
	public DefinitionRoot DefinitionRoot { get; }
	public ConstructorRecord ConstructorRecord { get; }

	public DefinitionResult(DefinitionRoot definitionRoot, ConstructorRecord constructorRecord)
	{
		DefinitionRoot = definitionRoot;
		ConstructorRecord = constructorRecord;
	}
}