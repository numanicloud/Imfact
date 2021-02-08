using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deptorygen2.Core.Structure;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Syntaxes.Parser
{
	internal class SingleResolverLoader
	{
		public ResolverSyntax? FromResolver(ResolverStructure item)
		{
			if (item.Symbol.ReturnType is not INamedTypeSymbol returnType)
			{
				return null;
			}

			return new ResolverSyntax(
				item.Symbol.Name,
				TypeName.FromSymbol(returnType),
				ResolutionSyntax.FromType(returnType),
				ResolutionSyntax.FromResolversAttribute(item.Symbol),
				ParameterSyntax.FromResolver(item.Symbol),
				item.Symbol.DeclaredAccessibility);
		}
	}
}
