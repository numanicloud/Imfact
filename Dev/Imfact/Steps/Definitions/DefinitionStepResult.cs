using Imfact.Interfaces;

namespace Imfact.Steps.Definitions
{
	internal class DefinitionStepResult
	{
		public DefinitionRoot DefinitionRoot { get; }
		public ConstructorRecord ConstructorRecord { get; }

		public DefinitionStepResult(DefinitionRoot definitionRoot, ConstructorRecord constructorRecord)
		{
			DefinitionRoot = definitionRoot;
			ConstructorRecord = constructorRecord;
		}
	}
}