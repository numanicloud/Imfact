using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Creation.Strategies.Template;
using Deptorygen2.Core.Steps.Semanticses.Nodes;

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
