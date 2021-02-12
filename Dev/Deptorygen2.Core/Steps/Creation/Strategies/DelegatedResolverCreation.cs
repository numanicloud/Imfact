using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Creation.Strategies
{
	internal class DelegatedResolverCreation : CreationMethodBase<DelegatedResolverCreation.Source>
	{
		public DelegatedResolverCreation(GenerationSemantics semantics) : base(semantics)
		{
		}

		protected override string GetCreationCode(Source resolution,
			GivenParameter[] given,
			ICreationAggregator aggregator)
		{
			var invocation = MethodInvocation(resolution.Resolver, given, aggregator);
			return $"{resolution.Delegation.PropertyName}.{invocation}";
		}

		protected override IEnumerable<Source> GetSource(GenerationSemantics semantics)
		{
			return semantics.Factory.Delegations.SelectMany(
				x => x.Resolvers.Select(y => new Source(x, y)));
		}

		protected override TypeName GetTypeInfo(Source source)
		{
			return source.Resolver.ReturnTypeName;
		}

		public record Source(DelegationSemantics Delegation, ResolverSemantics Resolver);
	}
}
