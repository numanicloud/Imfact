using System.Linq;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;

namespace Deptorygen2.Core.Steps.Expressions.Strategies
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
