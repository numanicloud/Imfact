using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Instantiation;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;
using IServiceProvider = Deptorygen2.Core.Interfaces.IServiceProvider;

namespace Deptorygen2.Core.Steps.Definitions
{
	/* やってること：
	 *		- 必要な依存関係（フィールド）の集計
	 *		- 必要な名前空間の集計
	 *		- （オブジェクト生成方法の決定）
	 *		- 各種データ型を、コード生成に必要な情報のみを持つものに変換
	 *
	 */

	internal class DefinitionAggregator
	{
		public SourceCodeDefinition Encode(GenerationSemantics semantics)
		{
			var factoryS = semantics.Factory;

			string[] usings = semantics.RequiredNamespaces;
			string ns = factoryS.ItselfSymbol.GetFullNameSpace();

			var factory = FactoryDefinition.Build(factoryS, name =>
			{
				var fields = AggregateDependencies(factoryS);

				var resolvers = AggregateResolvers(factoryS);
				var collectionResolvers = AggregateCollectionResolvers(factoryS);
				var delegations = factoryS.Delegations.Select(BuildDelegation).ToArray();

				var constructor = FactoryConstructorDefinition.Build(fields, delegations);

				return new FactoryDefinition(name, resolvers, collectionResolvers, delegations,
					fields, constructor);
			});

			var creation = new InstantiationResolver(semantics);
			
			return new SourceCodeDefinition(usings, ns, factory, creation);
		}

		private static CollectionResolverDefinition[] AggregateCollectionResolvers(
			FactorySemantics semantics)
		{
			return CollectionResolverDefinition.Build(semantics, (resolver, data) =>
			{
				var parameters = BuildParameters(resolver.Parameters);
				var resolutions = resolver.Resolutions
					.Select(r => BuildResolution(r))
					.ToArray();

				HookDefinition[] hooks = new HookDefinition[0]; // TODO: 未実装

				return new CollectionResolverDefinition(data, resolutions, parameters, hooks);
			}).ToArray();
		}

		private static ResolverDefinition[] AggregateResolvers(FactorySemantics semantics)
		{
			return ResolverDefinition.Build(semantics, (resolver, data) =>
			{
				var primalResolution = resolver.ReturnTypeResolution ?? resolver.Resolutions[0];
				var parameters = BuildParameters(resolver.Parameters);
				var resolution = BuildResolution(primalResolution);

				HookDefinition[] hooks = new HookDefinition[0]; // TODO: 未実装

				return new ResolverDefinition(data, resolution, parameters, hooks);
			}).ToArray();
		}

		private DelegationDefinition BuildDelegation(DelegationSemantics delegation)
		{
			return new(delegation.TypeName, delegation.PropertyName);
		}

		private static ResolverParameterDefinition[] BuildParameters(ParameterSemantics[] parameters)
		{
			return parameters
				.Select(p => new ResolverParameterDefinition(p.TypeName, p.ParameterName))
				.ToArray();
		}

		private static ResolutionDefinition BuildResolution(ResolutionSemantics resolution)
		{
			return new ResolutionDefinition(resolution.TypeName);
		}

		private static DependencyDefinition[] AggregateDependencies(FactorySemantics semantics)
		{
			var consumers = semantics.Resolvers.Cast<IServiceConsumer>()
				.Concat(semantics.CollectionResolvers)
				.SelectMany(x => x.GetRequiredServiceTypes())
				.Distinct();

			var providers = semantics.Resolvers.Cast<IServiceProvider>()
				.Concat(semantics.CollectionResolvers)
				.Concat(semantics.Delegations)
				.SelectMany(x => x.GetCapableServiceTypes())
				.Distinct();

			return consumers.Except(providers)
				.Select(t => new DependencyDefinition(t, "_" + t.Name.ToLowerCamelCase()))
				.ToArray();
		}

		private static IEnumerable<string> AggregateNamespaces(FactorySemantics semantics)
		{
			return semantics.Resolvers.Cast<INamespaceClaimer>()
				.Concat(semantics.CollectionResolvers)
				.Concat(semantics.Delegations)
				.SelectMany(x => x.GetRequiredNamespaces())
				.Distinct();
		}
	}
}
