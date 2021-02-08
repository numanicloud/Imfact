using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Syntaxes
{
	internal record FactorySyntax(INamedTypeSymbol ItselfSymbol,
		ResolverSyntax[] Resolvers,
		CollectionResolverSyntax[] CollectionResolvers,
		DelegationSyntax[] Delegations) : Interfaces.IServiceProvider
	{
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
