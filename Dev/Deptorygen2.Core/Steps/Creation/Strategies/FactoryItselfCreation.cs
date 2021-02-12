using System.Collections.Generic;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Instantiation.CreationMethods
{
	internal class FactoryItselfCreation : CreationMethodBase<FactorySemantics>
	{
		public FactoryItselfCreation(GenerationSemantics semantics) : base(semantics)
		{
		}

		protected override string GetCreationCode(FactorySemantics resolution,
			GivenParameter[] given,
			ICreationAggregator aggregator)
		{
			return "this";
		}

		protected override IEnumerable<FactorySemantics> GetSource(GenerationSemantics semantics)
		{
			return semantics.Factory.WrapByArray();
		}

		protected override TypeName GetTypeInfo(FactorySemantics source)
		{
			return TypeName.FromSymbol(source.ItselfSymbol);
		}
	}
}
