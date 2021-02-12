using System.Collections.Generic;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Creation.Strategies
{
	internal class FieldCreation : CreationMethodBase<DependencySemantics>
	{
		public FieldCreation(GenerationSemantics semantics) : base(semantics)
		{
		}

		protected override string GetCreationCode(DependencySemantics resolution, GivenParameter[] given,
			ICreationAggregator aggregator)
		{
			return resolution.FieldName;
		}

		protected override IEnumerable<DependencySemantics> GetSource(GenerationSemantics semantics)
		{
			return semantics.Dependencies;
		}

		protected override TypeName GetTypeInfo(DependencySemantics source)
		{
			return source.TypeName;
		}
	}
}
