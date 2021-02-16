using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Steps.Creation.Abstraction;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using Deptorygen2.Core.Utilities;
using InheritanceSemantics = Deptorygen2.Core.Steps.Semanticses.Nodes.Inheritance;

namespace Deptorygen2.Core.Steps.Creation.Strategies
{
	internal class InheritanceResolver : CreationMethodBase<InheritanceResolver.Source>
	{
		public InheritanceResolver(Generation semantics) : base(semantics)
		{
		}

		protected override string GetCreationCode(Source resolution, GivenParameter[] given,
			ICreationAggregator aggregator)
		{
			var invocation = MethodInvocation(resolution.Resolver, given, aggregator);
			return $"base.{invocation}";
		}

		protected override IEnumerable<Source> GetSource(Generation semantics)
		{
			return semantics.Factory.Inheritances.SelectMany(x => x.Resolvers
				.Select(y => new Source(x, y)));
		}

		protected override TypeName GetTypeInfo(Source source)
		{
			return source.Resolver.ReturnType;
		}

		public record Source(InheritanceSemantics Inheritance, Semanticses.Nodes.Resolver Resolver);
	}
}
