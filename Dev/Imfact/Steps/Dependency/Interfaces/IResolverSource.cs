using Imfact.Steps.Semanticses;
using Imfact.Steps.Semanticses.Interfaces;
using Imfact.Steps.Semanticses.Records;

namespace Imfact.Steps.Dependency.Interfaces
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