using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Creation.Abstraction;

namespace Deptorygen2.Core.Steps.Definitions
{
	internal class SourceTreeDefinition
	{
		public DefinitionRoot DefinitionRoot { get; }
		public ICreationAggregator Creation { get; }
		public ConstructorRecord ConstructorRecord { get; }

		public SourceTreeDefinition(DefinitionRoot definitionRoot, ICreationAggregator creation, ConstructorRecord constructorRecord)
		{
			DefinitionRoot = definitionRoot;
			Creation = creation;
			ConstructorRecord = constructorRecord;
		}
	}
}