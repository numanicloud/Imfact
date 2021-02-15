using System.Collections.Generic;
using Deptorygen2.Core.Steps.Creation.Abstraction;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Creation.Strategies
{
	internal class Resolver : CreationMethodBase<Semanticses.Nodes.Resolver>
	{
		public Resolver(Generation semantics) : base(semantics)
		{
		}
		
		protected override string GetCreationCode(Semanticses.Nodes.Resolver resolution,
			GivenParameter[] given,
			ICreationAggregator aggregator)
		{
			return MethodInvocation(resolution, given, aggregator);
		}

		protected override IEnumerable<Semanticses.Nodes.Resolver> GetSource(Generation semantics)
		{
			return semantics.Factory.Resolvers;
		}

		protected override TypeName GetTypeInfo(Semanticses.Nodes.Resolver source)
		{
			return source.ReturnType;
		}
	}
}
