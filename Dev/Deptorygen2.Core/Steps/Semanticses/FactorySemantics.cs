using System;
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

		public static FactorySemantics Build(ClassToAnalyze @class,
			Func<Partial, FactorySemantics> completion)
		{
			var partial = new Partial(@class.Symbol);
			return completion(partial);
		}

		public record Partial(INamedTypeSymbol ItselfSymbol)
		{
			public FactorySemantics Complete(ResolverSemantics[] resolvers,
				CollectionResolverSemantics[] collectionResolvers,
				DelegationSemantics[] delegations)
			{
				return new FactorySemantics(ItselfSymbol, resolvers, collectionResolvers,
					delegations);
			}
		}
	}
}
