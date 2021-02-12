using Deptorygen2.Core.Steps.Creation;

namespace Deptorygen2.Core.Steps.Definitions
{
	internal class SourceCodeDefinition
	{
		public string[] Usings { get; }
		public string Ns { get; }
		public FactoryDefinition Factory { get; }
		public ICreationAggregator Creation { get; }

		public SourceCodeDefinition(string[] usings, string @namespace,
			FactoryDefinition factory, ICreationAggregator creation)
		{
			Usings = usings;
			Ns = @namespace;
			Factory = factory;
			Creation = creation;
		}
	}
}