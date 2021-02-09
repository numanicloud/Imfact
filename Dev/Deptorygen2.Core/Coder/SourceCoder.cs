using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Syntaxes;
using Deptorygen2.Core.Utilities;
using IServiceProvider = Deptorygen2.Core.Interfaces.IServiceProvider;

namespace Deptorygen2.Core.Coder
{
	internal class SourceCoder
	{
		public SourceCodeDefinition Encode(FactorySyntax syntax)
		{
			string[] usings = AggregateNamespaces(syntax).ToArray();
			string ns = syntax.ItselfSymbol.GetFullNameSpace();

			var factory = FactoryDefinition.Build(syntax, name =>
			{
				var fields = AggregateDependencies(syntax);
				var argumentResolver = new ArgumentResolver(syntax, fields);

				var resolvers = AggregateResolvers(syntax, argumentResolver);
				var collectionResolvers = AggregateCollectionResolvers(syntax, argumentResolver);
				var delegations = syntax.Delegations.Select(BuildDelegation).ToArray();

				return new FactoryDefinition(name, resolvers, collectionResolvers, delegations,
					fields);
			});

			return new SourceCodeDefinition(usings, ns, factory);
		}

		private static CollectionResolverDefinition[] AggregateCollectionResolvers(FactorySyntax syntax, ArgumentResolver argumentResolver)
		{
			return CollectionResolverDefinition.Build(syntax, (resolver, data) =>
			{
				var parameters = BuildParameters(resolver.Parameters);
				var resolutions = resolver.Resolutions
					.Select(r => BuildResolution(r, argumentResolver, parameters))
					.ToArray();

				HookDefinition[] hooks = new HookDefinition[0]; // TODO: 未実装

				return new CollectionResolverDefinition(data, resolutions, parameters, hooks);
			}).ToArray();
		}

		private static ResolverDefinition[] AggregateResolvers(FactorySyntax syntax, ArgumentResolver argumentResolver)
		{
			return ResolverDefinition.Build(syntax, (resolver, data) =>
			{
				var primalResolution = resolver.ReturnTypeResolution ?? resolver.Resolutions[0];
				var parameters = BuildParameters(resolver.Parameters);
				var resolution = BuildResolution(primalResolution, argumentResolver, parameters);

				HookDefinition[] hooks = new HookDefinition[0]; // TODO: 未実装

				return new ResolverDefinition(data, resolution, parameters, hooks);
			}).ToArray();
		}

		private DelegationDefinition BuildDelegation(DelegationSyntax delegation)
		{
			return new(delegation.TypeName, delegation.PropertyName);
		}

		private static ResolverParameterDefinition[] BuildParameters(ParameterSyntax[] parameters)
		{
			return parameters
				.Select(p => new ResolverParameterDefinition(p.TypeName, p.ParameterName))
				.ToArray();
		}

		private static ResolutionDefinition BuildResolution(ResolutionSyntax resolution,
			ArgumentResolver argumentResolver,
			ResolverParameterDefinition[] parameters)
		{
			var type = resolution.TypeName;
			var arguments = argumentResolver.GetArgumentCodes(resolution.Dependencies, parameters);

			return new ResolutionDefinition(type, arguments);
		}

		private static DependencyDefinition[] AggregateDependencies(FactorySyntax syntax)
		{
			var consumers = syntax.Resolvers.Cast<IServiceConsumer>()
				.Concat(syntax.CollectionResolvers)
				.SelectMany(x => x.GetRequiredServiceTypes())
				.Distinct();

			var providers = syntax.Resolvers.Cast<IServiceProvider>()
				.Concat(syntax.CollectionResolvers)
				.Concat(syntax.Delegations)
				.SelectMany(x => x.GetCapableServiceTypes())
				.Distinct();

			return consumers.Except(providers)
				.Select(t => new DependencyDefinition(t, "_" + t.Name.ToLowerCamelCase()))
				.ToArray();
		}

		private static IEnumerable<string> AggregateNamespaces(FactorySyntax syntax)
		{
			return syntax.Resolvers.Cast<INamespaceClaimer>()
				.Concat(syntax.CollectionResolvers)
				.Concat(syntax.Delegations)
				.SelectMany(x => x.GetRequiredNamespaces())
				.Distinct();
		}
	}
}
