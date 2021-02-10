using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Parser
{
	internal class SourceLoader
	{
		private readonly FactoryLoader _factoryLoader;
		private readonly ResolverLoader _resolverLoader;
		private readonly SingleResolverLoader _singleResolverLoader;
		private readonly CollectionResolverLoader _collectionResolverLoader;
		private readonly DelegationLoader _delegationLoader;
		private readonly ResolutionLoader _resolutionLoader;
		private readonly ParameterLoader _parameterLoader;

		public SourceLoader(IAnalysisContext context)
		{
			_factoryLoader = new FactoryLoader(context);

			_resolutionLoader = new ResolutionLoader(attr =>
				attr.AttributeClass?.Name == nameof(ResolutionAttribute)
				&& attr.ConstructorArguments.Length == 1,
				context);

			_resolverLoader = new ResolverLoader(method =>
				method.Syntax.IsPartial() && method.Symbol.MethodKind == MethodKind.Ordinary,
				_resolutionLoader,
				context);

			_collectionResolverLoader = new CollectionResolverLoader(
				method => SatisfyCollectionResolverCondition(method),
				_resolutionLoader);

			_singleResolverLoader = new SingleResolverLoader(
				method => !SatisfyCollectionResolverCondition(method),
				_resolutionLoader);

			_delegationLoader = new DelegationLoader(property =>
				Helpers.HasAttribute(property.TypeSymbol, nameof(FactoryAttribute))
				&& property.Symbol.IsReadOnly);

			_parameterLoader = new ParameterLoader(context);
		}

		private bool SatisfyCollectionResolverCondition(ResolverAnalysisContext method)
		{
			if (!IsCollectionType(method.Symbol.ReturnType))
			{
				return false;
			}

			var typeArgument = TypeName.FromSymbol(method.Symbol.ReturnType).TypeArguments[0];

			return method.Resolutions.Any(r => CanBeElement(r.Symbol, typeArgument));

			static bool IsCollectionType(ITypeSymbol symbol)
			{
				var type = TypeName.FromSymbol(symbol);
				var enumerableType = TypeName.FromType(typeof(IEnumerable<>));

				return type.NameWithoutArguments != enumerableType.NameWithoutArguments
					   && type.TypeArguments.Length != enumerableType.TypeArguments.Length;
			}

			static bool CanBeElement(INamedTypeSymbol nts, TypeName elementType)
			{
				return nts.AllInterfaces.Append(nts.BaseType).Append(nts)
					.FilterNull()
					.Any(x => TypeName.FromSymbol(x) == elementType);
			}
		}
	}

	internal record FactContext(FactoryFact Factory,
		ResolverFact[] Resolvers);
}
