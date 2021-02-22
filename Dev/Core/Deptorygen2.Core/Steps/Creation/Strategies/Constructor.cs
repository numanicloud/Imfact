using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Steps.Creation.Abstraction;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using Deptorygen2.Core.Utilities;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Creation.Strategies
{
	internal class Constructor : CreationMethodBase<Resolution>
	{
		public Constructor(SemanticsRoot semantics) : base(semantics)
		{
		}

		protected override string GetCreationCode(Resolution resolution, GivenParameter[] given,
			ICreationAggregator aggregator)
		{
			var request = new MultipleCreationRequest(
				resolution.Dependencies, given, false);
			return $"new {resolution.TypeName.Name}({GetArgList(request, aggregator)})";
		}

		protected override IEnumerable<Resolution> GetSource(SemanticsRoot semantics)
		{
			var rr = semantics.Factory.Resolvers.Select(x => x.ReturnTypeResolution).FilterNull();
			var rs = semantics.Factory.Resolvers.SelectMany(x => x.Resolutions);
			var crs = semantics.Factory.MultiResolvers.SelectMany(x => x.Resolutions);
			return rr.Concat(rs).Concat(crs)
				.GroupBy(x => x.TypeName)
				.Select(x => x.First());
		}

		protected override TypeRecord GetTypeInfo(Resolution source)
		{
			return source.TypeName.Record;
		}
	}
}
