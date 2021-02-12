using System.Collections.Generic;
using Deptorygen2.Core.Steps.Aggregation;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Steps.Semanticses
{
	internal record FactorySemantics(INamedTypeSymbol ItselfSymbol,
		ResolverSemantics[] Resolvers,
		CollectionResolverSemantics[] CollectionResolvers,
		DelegationSemantics[] Delegations) : Interfaces.IServiceProvider
	{
		public IEnumerable<TypeName> GetCapableServiceTypes()
		{
			yield return TypeName.FromSymbol(ItselfSymbol);
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

			return new(@class, tuple => new FactorySemantics(
				@class.Symbol, tuple.Item1, tuple.Item2, tuple.Item3));
		}
	}
}
