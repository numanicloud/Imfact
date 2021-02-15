using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Steps.Creation.Abstraction;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Creation.Strategies
{
	internal class DelegatedMultiResolver : CreationMethodBase<DelegatedMultiResolver.Source>
	{
		public DelegatedMultiResolver(Generation semantics) : base(semantics)
		{
		}

		protected override string GetCreationCode(Source resolution,
			GivenParameter[] given, ICreationAggregator aggregator)
		{
			var invocation = MethodInvocation(resolution.MultiResolver, given, aggregator);
			return $"{resolution.Delegation.PropertyName}.{invocation}";
		}

		protected override IEnumerable<Source> GetSource(Generation semantics)
		{
			return semantics.Factory.Delegations.SelectMany(
				x => x.CollectionResolvers.Select(y => new Source(x, y)));
		}

		protected override TypeName GetTypeInfo(Source source)
		{
			return source.MultiResolver.ReturnType;
		}

		public record Source(Delegation Delegation,
			Semanticses.Nodes.MultiResolver MultiResolver);
	}
}
