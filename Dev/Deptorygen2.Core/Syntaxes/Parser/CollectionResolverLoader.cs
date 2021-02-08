using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Syntaxes.Parser
{
	internal class CollectionResolverLoader
	{
		/* 仕様都合の条件：
		 *		IEnumerable<T> 型である
		 *		Resolution属性で指定された型のうち、T に代入できるものが1つ以上ある
		 */

		public CollectionResolverSyntax? FromResolver(IMethodSymbol resolver)
		{
			if (TryGetIEnumerableType(resolver.ReturnType) is not ({ } collection, { } element))
			{
				return null;
			}

			var resolutions = ResolutionSyntax.FromResolversAttribute(resolver,
				x => IsElementType(x, element) ? ResolutionSyntax.FromType(x) : null);

			if (resolutions.Any())
			{
				var parameters = ParameterSyntax.FromResolver(resolver);
				return new CollectionResolverSyntax(resolver.Name, collection, parameters, resolutions);
			}

			return null;
		}

		private (TypeName? collection, TypeName? element) TryGetIEnumerableType(ITypeSymbol symbol)
		{
			var type = TypeName.FromSymbol(symbol);

			if (type.NameWithoutArguments != "IEnumerable" || type.TypeArguments.Length != 1)
			{
				return (null, null);
			}

			return (type, type.TypeArguments[0]);
		}

		private bool IsElementType(INamedTypeSymbol nts, TypeName elementType)
		{
			return nts.AllInterfaces.Append(nts.BaseType).Append(nts)
				.FilterNull()
				.Any(x => TypeName.FromSymbol(x) == elementType);
		}
	}
}
