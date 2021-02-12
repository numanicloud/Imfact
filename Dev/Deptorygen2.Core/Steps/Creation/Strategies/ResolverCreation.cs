using System.Collections.Generic;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Instantiation.CreationMethods
{
	internal class ResolverCreation : CreationMethodBase<ResolverSemantics>
	{
		public ResolverCreation(GenerationSemantics semantics) : base(semantics)
		{
		}
		
		protected override string GetCreationCode(ResolverSemantics resolution,
			GivenParameter[] given,
			ICreationAggregator aggregator)
		{
			return MethodInvocation(resolution, given, aggregator);
		}

		protected override IEnumerable<ResolverSemantics> GetSource(GenerationSemantics semantics)
		{
			return semantics.Factory.Resolvers;
		}

		protected override TypeName GetTypeInfo(ResolverSemantics source)
		{
			return source.ReturnTypeName;
		}
	}
}
