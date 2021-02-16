using System.Collections.Generic;
using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Steps.Creation.Abstraction;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Creation.Strategies
{
	internal class Field : CreationMethodBase<Dependency>
	{
		public Field(Generation semantics) : base(semantics)
		{
		}

		protected override string GetCreationCode(Dependency resolution, GivenParameter[] given,
			ICreationAggregator aggregator)
		{
			return resolution.FieldName;
		}

		protected override IEnumerable<Dependency> GetSource(Generation semantics)
		{
			return semantics.Dependencies;
		}

		protected override TypeRecord GetTypeInfo(Dependency source)
		{
			return source.TypeName.Record;
		}
	}
}
