using System.Collections.Generic;
using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Steps.Creation.Abstraction;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Creation.Strategies
{
	internal class DelegationItself : CreationMethodBase<Delegation>
	{
		public DelegationItself(Generation semantics) : base(semantics)
		{
		}

		protected override string GetCreationCode(Delegation resolution,
			GivenParameter[] given,
			ICreationAggregator aggregator)
		{
			return resolution.PropertyName;
		}

		protected override IEnumerable<Delegation> GetSource(Generation semantics)
		{
			return semantics.Factory.Delegations;
		}

		protected override TypeRecord GetTypeInfo(Delegation source)
		{
			return source.Type.Record;
		}
	}
}
