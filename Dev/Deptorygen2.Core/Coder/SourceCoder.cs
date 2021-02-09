using System;
using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Syntaxes;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using IServiceProvider = Deptorygen2.Core.Interfaces.IServiceProvider;
using FactoryPartial = System.ValueTuple<
		Deptorygen2.Core.DependencyDefinition[],
		Deptorygen2.Core.ResolverDefinition[],
		Deptorygen2.Core.CollectionResolverDefinition[],
		Deptorygen2.Core.DelegationDefinition[]>;
using ResolverPartial = System.ValueTuple<
		Deptorygen2.Core.ResolverParameterDefinition[],
		Deptorygen2.Core.ResolutionDefinition,
		Deptorygen2.Core.HookDefinition[]>;
using CollectionResolverPartial = System.ValueTuple<
		Deptorygen2.Core.ResolverParameterDefinition[],
		Deptorygen2.Core.ResolutionDefinition[],
		Deptorygen2.Core.HookDefinition[]>;

namespace Deptorygen2.Core.Coder
{
	internal class SourceCoder
	{
		public SourceCodeDefinition Encode(FactorySyntax syntax)
		{
			string[] usings = AggregateNamespaces(syntax).ToArray();
			string ns = syntax.ItselfSymbol.GetFullNameSpace();

			var factory = BuildFactory(syntax, _ =>
			{
				var fields = BuildDependencies(syntax);

				var argumentResolver = new ArgumentResolver(syntax, fields);
				var methods = BuildResolvers(syntax, resolver =>
				{
					var primalResolution = resolver.ReturnTypeResolution ?? resolver.Resolutions[0];
					var parameters = BuildParameters(resolver.Parameters);
					var resolution = BuildResolution(primalResolution, argumentResolver, parameters);

					HookDefinition[] hooks = new HookDefinition[0]; // TODO: 未実装

					return (parameters, resolution, hooks);
				}).ToArray();

				var collectionResolvers = BuildCollectionResolvers(syntax, resolver =>
				{
					var parameters = BuildParameters(resolver.Parameters);
					var resolutions = resolver.Resolutions
						.Select(r => BuildResolution(r, argumentResolver, parameters))
						.ToArray();

					HookDefinition[] hooks = new HookDefinition[0]; // TODO: 未実装

					return (parameters, resolutions, hooks);
				}).ToArray();

				var delegations = syntax.Delegations.Select(BuildDelegation).ToArray();

				return (fields, methods, collectionResolvers, delegations);
			});

			return new SourceCodeDefinition(usings, ns, factory);
		}

		private DelegationDefinition BuildDelegation(DelegationSyntax delegation)
		{
			return new DelegationDefinition(delegation.TypeName, delegation.PropertyName);
		}

		private FactoryDefinition BuildFactory(FactorySyntax syntax,
			Func<FactorySyntax, FactoryPartial> loader)
		{
			var name = syntax.ItselfSymbol.Name;
			var (fields, methods, collectionResolvers, delegations) = loader(syntax);
			return new FactoryDefinition(name, methods, collectionResolvers, delegations, fields);
		}

		private static DependencyDefinition[] BuildDependencies(FactorySyntax syntax)
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

		private static ResolverParameterDefinition[] BuildParameters(ParameterSyntax[] parameters)
		{
			return parameters
				.Select(p => new ResolverParameterDefinition(p.TypeName, p.ParameterName))
				.ToArray();
		}

		private static ResolutionDefinition BuildResolution(ResolverSyntax resolver,
			ArgumentResolver argumentResolver,
			ResolverParameterDefinition[] parameters)
		{
			var source = resolver.ReturnTypeResolution ?? resolver.Resolutions[0];
			return BuildResolution(source, argumentResolver, parameters);
		}

		private static ResolutionDefinition BuildResolution(ResolutionSyntax resolution,
			ArgumentResolver argumentResolver,
			ResolverParameterDefinition[] parameters)
		{
			var type = resolution.TypeName;
			var arguments = argumentResolver.GetArgumentCodes(resolution.Dependencies, parameters);

			return new ResolutionDefinition(type, arguments);
		}

		private IEnumerable<ResolverDefinition> BuildResolvers(FactorySyntax factory,
			Func<ResolverSyntax, ResolverPartial> loader)
		{
			return factory.Resolvers.Select(resolver =>
			{
				string name = resolver.MethodName;
				Accessibility accessLevel = resolver.Accessibility;
				TypeName returnType = resolver.ReturnTypeName;

				var (parameters, resolution, hooks) = loader(resolver);

				return new ResolverDefinition(accessLevel, name, returnType, resolution, parameters, hooks);
			});
		}

		private IEnumerable<CollectionResolverDefinition> BuildCollectionResolvers(FactorySyntax factory,
			Func<CollectionResolverSyntax, CollectionResolverPartial> loader)
		{
			return factory.CollectionResolvers.Select(resolver =>
			{
				string name = resolver.MethodName;
				Accessibility accessLevel = resolver.Accessibility;
				TypeName returnType = resolver.CollectionType;

				var (parameters, resolutions, hooks) = loader(resolver);

				return new CollectionResolverDefinition(accessLevel, name, returnType, resolutions, parameters, hooks);
			});
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
