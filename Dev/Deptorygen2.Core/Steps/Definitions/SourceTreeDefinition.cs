using Deptorygen2.Core.Steps.Creation;

namespace Deptorygen2.Core.Steps.Definitions.Syntaxes
{
	internal class SourceTreeDefinition
	{
		public RootNode Root { get; }
		public ICreationAggregator Creation { get; }

		public SourceTreeDefinition(RootNode root, ICreationAggregator creation)
		{
			Root = root;
			Creation = creation;
		}
	}
}