using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.PlatformServices;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Aspects.Nodes;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using NacHelpers.Extensions;
using Attribute = Deptorygen2.Core.Steps.Aspects.Nodes.Attribute;
using Parameter = Deptorygen2.Core.Steps.Semanticses.Nodes.Parameter;

namespace Deptorygen2.Core.Steps.Semanticses
{
	internal class SemanticsAggregator
	{
		private readonly IAnalysisContext _context;

		public SemanticsAggregator(IAnalysisContext context)
		{
			_context = context;
		}

		public Generation? Aggregate(Class @class, IAnalysisContext context)
		{
			var methods = @class.GetMethods(context);
			var properties = @class.GetProperties(context);

			return Generation.GetBuilder(@class).Build(_ =>
			{
				var factory = Factory.GetBuilder(@class)?.Build(_ =>
				{
					var resolvers = AggregateResolvers(methods);
					var collectionResolvers = AggregateCollectionResolvers(methods);

					var delegations = properties.Select(Delegation.GetBuilder).Build(p =>
					{
						var ms = p.GetMethodToAnalyze();
						var dr = AggregateResolvers(ms);
						var dcr = AggregateCollectionResolvers(ms);
						return (dr, dcr);
					});

					var entries = resolvers.Select(EntryResolver.FromResolver).ToArray();

					return (resolvers, collectionResolvers, delegations, entries);
				});

				if (factory is null)
				{
					return (new string[0], null, new Dependency[0]);
				}

				var dependencies = Dependency.FromFactory(factory);
				var namespaces = AggregateNamespaces(factory, dependencies).ToArray();

				return (namespaces, factory, dependencies);
			});
		}

		private Resolver[] AggregateResolvers(Method[] methods)
		{
			return methods.Select(Resolver.GetBuilder).Build(m =>
			{
				var attr = m.GetAttributes();

				var ret = m.GetReturnType(_context) is { } t
					? Resolution.Build(t, _context)
					: null;
				var (parameters, resolutions, hooks) = LoadMethodFeature(m.GetParameters(), attr, m.Symbol.Name);

				return (ret, resolutions, parameters, hooks);
			});
		}

		private MultiResolver[] AggregateCollectionResolvers(Method[] methods)
		{
			return methods.Select(MultiResolver.GetBuilder).Build(
				m => LoadMethodFeature(m.GetParameters(), m.GetAttributes(), m.Symbol.Name));
		}

		private (Parameter[], Resolution[], Hook[]) LoadMethodFeature(
			Aspects.Nodes.Parameter[] parameters, Attribute[] attributes, string methodName)
		{
			var ps = Build(parameters, p => Parameter.Build(p, _context));
			var rs = Build(attributes, a => Resolution.Build(a, _context));
			var hs = Build(attributes, a => Hook.Build(a, _context, methodName));
			return (ps, rs, hs);
		}

		private TResult[] Build<T, TResult>(IEnumerable<T> source, Func<T, TResult?> selector)
			where TResult : class
		{
			return source.Select(selector)
				.FilterNull()
				.ToArray();
		}

		private static IEnumerable<string> AggregateNamespaces(Factory semantics,
			Dependency[] dependencies)
		{
			return semantics.Resolvers.Cast<INamespaceClaimer>()
				.Concat(semantics.CollectionResolvers)
				.Concat(semantics.Delegations)
				.Concat(semantics.Resolvers.SelectMany(x => x.Hooks))
				.Concat(dependencies)
				.SelectMany(x => x.GetRequiredNamespaces())
				.Except(semantics.Type.FullNamespace.WrapByArray())
				.Append("System.ComponentModel")
				.Distinct();
		}
	}
}
