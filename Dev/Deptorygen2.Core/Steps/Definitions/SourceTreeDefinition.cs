using Deptorygen2.Core.Steps.Creation.Abstraction;

namespace Deptorygen2.Core.Steps.Definitions
{
	internal class SourceTreeDefinition
	{
		public DefinitionRoot DefinitionRoot { get; }
		public ICreationAggregator Creation { get; }

		public SourceTreeDefinition(DefinitionRoot definitionRoot, ICreationAggregator creation)
		{
			DefinitionRoot = definitionRoot;
			Creation = creation;
		}
	}
}