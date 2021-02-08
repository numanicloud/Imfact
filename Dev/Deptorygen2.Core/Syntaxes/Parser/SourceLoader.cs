using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deptorygen2.Core.Structure;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Syntaxes.Parser
{
	internal class SourceLoader
	{
		private readonly FactoryLoader _factoryLoader = new();
		private readonly ResolverLoader _resolverLoader;
		private readonly SingleResolverLoader _singleResolverLoader;
		private readonly CollectionResolverLoader _collectionResolverLoader;
		private readonly DelegationLoader _delegationLoader;
		private readonly ResolutionLoader _resolutionLoader;

		public SourceLoader()
		{
			_resolutionLoader = new ResolutionLoader(attr =>
				attr.AttributeClass?.Name == nameof(ResolutionAttribute)
				&& attr.ConstructorArguments.Length == 1);

			_resolverLoader = new ResolverLoader(method =>
				method.Syntax.IsPartial() && method.Symbol.MethodKind == MethodKind.Ordinary,
				_resolutionLoader);

			_collectionResolverLoader = new CollectionResolverLoader(
				method => SatisfyCollectionResolverCondition(method),
				_resolutionLoader);

			_singleResolverLoader = new SingleResolverLoader(
				method => !SatisfyCollectionResolverCondition(method),
				_resolutionLoader);

			_delegationLoader = new DelegationLoader(property =>
				property.TypeSymbol.HasAttribute(nameof(FactoryAttribute))
				&& property.Symbol.IsReadOnly);
		}

		public async Task<FactorySyntax> LoadAsync(
			ClassDeclarationSyntax classDeclarationSyntax,
			SourceGenAnalysisContext context)
		{
			return await _factoryLoader.BuildFactorySyntaxAsync(classDeclarationSyntax, context,
				(factory, singles, collections, delegations) =>
				{
					LoadResolvers(factory, singles, collections);

					_delegationLoader.BuildDelegationSyntaxes(factory,
							(delegated, delegatedResolvers, delegatedCollectionResolvers) =>
							{
								LoadResolvers(delegated, delegatedResolvers, delegatedCollectionResolvers);
							})
						.AddRangeTo(delegations);
				});

			void LoadResolvers(FactoryAnalysisContext factory,
				List<ResolverSyntax> singles,
				List<CollectionResolverSyntax> collections)
			{
				var methods = _resolverLoader.BuildResolverStructures(factory,
					(symbol, resolutionList) =>
					{
						_resolutionLoader.GetStructures(symbol).AddRangeTo(resolutionList);
					}).ToArray();

				methods.Select(m => _singleResolverLoader.FromStructure(m))
					.FilterNull()
					.AddRangeTo(singles);

				methods.Select(s => _collectionResolverLoader.FromResolver(s))
					.FilterNull()
					.AddRangeTo(collections);
			}
		}

		private bool SatisfyCollectionResolverCondition(ResolverStructure method)
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
}
