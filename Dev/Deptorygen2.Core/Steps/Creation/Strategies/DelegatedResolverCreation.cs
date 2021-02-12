using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Instantiation.CreationMethods
{
	internal class DelegatedResolverCreation : CreationMethodBase<(DelegationSemantics, ResolverSemantics)>
	{
		public DelegatedResolverCreation(GenerationSemantics semantics) : base(semantics)
		{
		}

		protected override string GetCreationCode((DelegationSemantics, ResolverSemantics) resolution,
			GivenParameter[] given,
			ICreationAggregator aggregator)
		{
			var invocation = MethodInvocation(resolution.Item2, given, aggregator);
			return $"{resolution.Item1.PropertyName}.{invocation}";
		}

		protected override IEnumerable<(DelegationSemantics, ResolverSemantics)> GetSource(GenerationSemantics semantics)
		{
			return semantics.Factory.Delegations
				.SelectMany(x => x.Resolvers.Select(y => (x, y)));
		}

		protected override TypeName GetTypeInfo((DelegationSemantics, ResolverSemantics) source)
		{
			return source.Item2.ReturnTypeName;
		}
	}
}
