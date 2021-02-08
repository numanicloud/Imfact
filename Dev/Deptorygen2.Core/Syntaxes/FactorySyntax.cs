using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Syntaxes
{
	class FactorySyntax : Interfaces.IServiceProvider
	{
		public INamedTypeSymbol ItselfSymbol { get; }
		public ResolverSyntax[] Resolvers { get; }
		public CollectionResolverSyntax[] CollectionResolvers { get; }
		public DelegationSyntax[] Delegations { get; }

		public FactorySyntax(INamedTypeSymbol itselfSymbol,
			ResolverSyntax[] resolvers,
			CollectionResolverSyntax[] collectionResolvers,
			DelegationSyntax[] delegations)
		{
			ItselfSymbol = itselfSymbol;
			Resolvers = resolvers;
			CollectionResolvers = collectionResolvers;
			Delegations = delegations;
		}

		public IEnumerable<TypeName> GetCapableServiceTypes()
		{
			yield return TypeName.FromSymbol(ItselfSymbol);
			foreach (var delegation in Delegations)
			{
				yield return delegation.TypeName;
			}
		}
	}
}
