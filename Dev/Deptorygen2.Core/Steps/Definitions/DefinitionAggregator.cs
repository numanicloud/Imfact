using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;
using IServiceProvider = Deptorygen2.Core.Interfaces.IServiceProvider;

namespace Deptorygen2.Core.Steps.Definitions
{
	internal class DefinitionAggregator
	{
		public SourceCodeDefinition Encode(FactorySemantics semantics)
		{
			string[] usings = AggregateNamespaces(semantics).ToArray();
			string ns = semantics.ItselfSymbol.GetFullNameSpace();

			var factory = FactoryDefinition.Build(semantics, name =>
			{
				var fields = AggregateDependencies(semantics);
				var argumentResolver = new ArgumentResolver(semantics, fields);

				var resolvers = AggregateResolvers(semantics, argumentResolver);
				var collectionResolvers = AggregateCollectionResolvers(semantics, argumentResolver);
				var delegations = semantics.Delegations.Select(BuildDelegation).ToArray();

				return new FactoryDefinition(name, resolvers, collectionResolvers, delegations,
					fields);
			});

			return new SourceCodeDefinition(usings, ns, factory);
		}

		private static CollectionResolverDefinition[] AggregateCollectionResolvers(FactorySemantics semantics, ArgumentResolver argumentResolver)
		{
			return CollectionResolverDefinition.Build(semantics, (resolver, data) =>
			{
				var parameters = BuildParameters(resolver.Parameters);
				var resolutions = resolver.Resolutions
					.Select(r => BuildResolution(r, argumentResolver, parameters))
					.ToArray();

				HookDefinition[] hooks = new HookDefinition[0]; // TODO: 未実装

				return new CollectionResolverDefinition(data, resolutions, parameters, hooks);
			}).ToArray();
		}

		private static ResolverDefinition[] AggregateResolvers(FactorySemantics semantics, ArgumentResolver argumentResolver)
		{
			return ResolverDefinition.Build(semantics, (resolver, data) =>
			{
				var primalResolution = resolver.ReturnTypeResolution ?? resolver.Resolutions[0];
				var parameters = BuildParameters(resolver.Parameters);
				var resolution = BuildResolution(primalResolution, argumentResolver, parameters);

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

		private static ResolutionDefinition BuildResolution(ResolutionSemantics resolution,
			ArgumentResolver argumentResolver,
			ResolverParameterDefinition[] parameters)
		{
			var type = resolution.TypeName;
			var arguments = argumentResolver.GetArgumentCodes(resolution.Dependencies, parameters);

			return new ResolutionDefinition(type, arguments);
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
