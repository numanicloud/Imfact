using Deptorygen2.Core.Steps.Instantiation;

namespace Deptorygen2.Core.Steps.Definitions
{
	internal class SourceCodeDefinition
	{
		public string[] Usings { get; }
		public string Ns { get; }
		public FactoryDefinition Factory { get; }

		public SourceCodeDefinition(string[] usings, string @namespace,
			FactoryDefinition factory)
		{
			Usings = usings;
			Ns = @namespace;
			Factory = factory;
		}
	}
}