using Deptorygen2.Core.Interfaces;

namespace Deptorygen2.Core.Steps.Definitions
{
	internal class SourceTreeDefinition
	{
		public DefinitionRoot DefinitionRoot { get; }
		public ConstructorRecord ConstructorRecord { get; }

		public SourceTreeDefinition(DefinitionRoot definitionRoot, ConstructorRecord constructorRecord)
		{
			DefinitionRoot = definitionRoot;
			ConstructorRecord = constructorRecord;
		}
	}
}