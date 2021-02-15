using System.Collections.Generic;
using Deptorygen2.Core.Steps.Creation.Abstraction;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using Deptorygen2.Core.Utilities;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Creation.Strategies
{
	internal class FactoryItself : CreationMethodBase<Factory>
	{
		public FactoryItself(Generation semantics) : base(semantics)
		{
		}

		protected override string GetCreationCode(Factory resolution,
			GivenParameter[] given,
			ICreationAggregator aggregator)
		{
			return "this";
		}

		protected override IEnumerable<Factory> GetSource(Generation semantics)
		{
			return semantics.Factory.WrapByArray();
		}

		protected override TypeName GetTypeInfo(Factory source)
		{
			return source.Type;
		}
	}
}
