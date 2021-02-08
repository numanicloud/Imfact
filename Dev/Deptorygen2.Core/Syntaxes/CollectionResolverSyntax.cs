using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Syntaxes
{
	class CollectionResolverSyntax : IServiceConsumer, IServiceProvider
	{
		public string MethodName { get; }
		public TypeName CollectionType { get; }
		public TypeName ElementType => CollectionType.TypeArguments[0];
		public ParameterSyntax[] Parameters { get; }
		public ResolutionSyntax[] Resolutions { get; }

		public CollectionResolverSyntax(string methodName,
			TypeName collectionType,
			ParameterSyntax[] parameters,
			ResolutionSyntax[] resolutions)
		{
			MethodName = methodName;
			CollectionType = collectionType;
			Parameters = parameters;
			Resolutions = resolutions;
		}

		public static CollectionResolverSyntax? FromResolver(IMethodSymbol resolver)
		{
			if (TryGetIEnumerableType(resolver.ReturnType) is not ({} collection, {} element))
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

		private static (TypeName? collection, TypeName? element) TryGetIEnumerableType(ITypeSymbol symbol)
		{
			var type = TypeName.FromSymbol(symbol);

			if (type.NameWithoutArguments != "IEnumerable"
				|| type.TypeArguments.Length != 1)
			{
				return (null, null);
			}

			return (type, type.TypeArguments[0]);
		}

		private static bool IsElementType(INamedTypeSymbol nts, TypeName elementType)
		{
			return nts.AllInterfaces.Append(nts.BaseType).Append(nts)
				.FilterNull()
				.Any(x => TypeName.FromSymbol(x) == elementType);
		}

		public IEnumerable<TypeName> GetRequiredServiceTypes()
		{
			return Resolutions.SelectMany(x => x.Dependencies)
				.Except(Parameters.Select(x => x.TypeName));
		}

		public IEnumerable<TypeName> GetCapableServiceTypes()
		{
			yield return CollectionType;
		}
	}
}
