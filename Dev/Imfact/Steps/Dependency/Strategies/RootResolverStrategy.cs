using System.Linq;
using Imfact.Steps.Dependency.Interfaces;
using Imfact.Steps.Semanticses;
using Imfact.Steps.Semanticses.Interfaces;

namespace Imfact.Steps.Dependency.Strategies
{
	internal class RootResolverStrategy : FactoryExpressionStrategy<Factory, Resolver>
	{
		public RootResolverStrategy(IFactorySource<Factory> factorySource, IResolverSource<Resolver> resolverSource)
			: base(factorySource, resolverSource)
		{
		}

		protected override Source[] Filter(Source[] source, IResolverSemantics caller)
		{
			return source
				.Where(x => caller is not Resolver r || x.Resolver.GetHashCode() != r.GetHashCode())
				.ToArray();
		}
	}
}
