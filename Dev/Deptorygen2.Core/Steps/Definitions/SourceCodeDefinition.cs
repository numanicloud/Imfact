using Deptorygen2.Core.Steps.Instantiation;

namespace Deptorygen2.Core.Steps.Definitions
{
	internal class SourceCodeDefinition
	{
		public string[] Usings { get; }
		public string Ns { get; }
		public FactoryDefinition Factory { get; }
		public IInstantiationResolver Creation { get; }

		public SourceCodeDefinition(string[] usings, string @namespace,
			FactoryDefinition factory, IInstantiationResolver creation)
		{
			Usings = usings;
			Ns = @namespace;
			Factory = factory;
			Creation = creation;
		}
	}
}