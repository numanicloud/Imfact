using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;

namespace Deptorygen2.Core.Steps.Expressions.Strategies
{
	interface IResolverSource<out T> where T : IResolverSemantics
	{
		T[] GetResolverSource(IFactorySemantics semantics);
	}
	class ResolverSource : IResolverSource<Resolver>
	{
		public Resolver[] GetResolverSource(IFactorySemantics semantics)
		{
			return semantics.Resolvers;
		}
	}

	class MultiResolverSource : IResolverSource<MultiResolver>
	{
		public MultiResolver[] GetResolverSource(IFactorySemantics semantics)
		{
			return semantics.MultiResolvers;
		}
	}
}