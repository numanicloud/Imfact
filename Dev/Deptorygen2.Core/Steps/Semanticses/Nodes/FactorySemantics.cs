using System.Collections.Generic;
using Deptorygen2.Core.Steps.Aggregation;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Semanticses
{
	internal record FactorySemantics(TypeName Type,
		ResolverSemantics[] Resolvers,
		CollectionResolverSemantics[] CollectionResolvers,
		DelegationSemantics[] Delegations) : Interfaces.IServiceProvider
	{
		public IEnumerable<TypeName> GetCapableServiceTypes()
		{
			yield return Type;
			foreach (var delegation in Delegations)
			{
				yield return delegation.TypeName;
			}
		}

		public static Builder<ClassToAnalyze,
			(ResolverSemantics[],
			CollectionResolverSemantics[],
			DelegationSemantics[]),
			FactorySemantics>? GetBuilder(ClassToAnalyze @class)
		{
			if (!@class.IsFactory())
			{
				return null;
			}

			var t = TypeName.FromSymbol(@class.Symbol);

			return new(@class, tuple => new FactorySemantics(
				t, tuple.Item1, tuple.Item2, tuple.Item3));
		}
	}
}
