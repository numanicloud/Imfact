using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;

namespace Deptorygen2.Core.Steps.Creation.Strategies.Template
{
	interface IResolverSource<out T> where T : IResolverSemantics
	{
		T[] GetResolverSource(IFactorySemantics semantics);
	}
	class ResolverSource : IResolverSource<Semanticses.Nodes.Resolver>
	{
		public Semanticses.Nodes.Resolver[] GetResolverSource(IFactorySemantics semantics)
		{
			return semantics.Resolvers;
		}
	}

	class MultiResolverSource : IResolverSource<Semanticses.Nodes.MultiResolver>
	{
		public Semanticses.Nodes.MultiResolver[] GetResolverSource(IFactorySemantics semantics)
		{
			return semantics.MultiResolvers;
		}
	}
}