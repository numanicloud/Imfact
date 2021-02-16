using System.Collections.Generic;
using Deptorygen2.Core.Steps.Creation.Abstraction;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Creation.Strategies
{
	internal class MultiResolver : CreationMethodBase<Semanticses.Nodes.MultiResolver>
	{
		public MultiResolver(Generation semantics) : base(semantics)
		{
		}

		protected override string GetCreationCode(Semanticses.Nodes.MultiResolver resolution, GivenParameter[] given,
			ICreationAggregator aggregator)
		{
			return MethodInvocation(resolution, given, aggregator);
		}

		protected override IEnumerable<Semanticses.Nodes.MultiResolver> GetSource(Generation semantics)
		{
			return semantics.Factory.MultiResolvers;
		}

		protected override TypeName GetTypeInfo(Semanticses.Nodes.MultiResolver source)
		{
			return source.ReturnType;
		}
	}
}
