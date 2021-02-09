using System.Collections.Generic;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Syntaxes
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
	}
}
