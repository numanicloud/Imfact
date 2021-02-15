using System;
using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Aggregation;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Semanticses
{
	internal class SemanticsAggregator
	{
		private readonly IAnalysisContext _context;

		public SemanticsAggregator(IAnalysisContext context)
		{
			_context = context;
		}

		public GenerationSemantics? Aggregate(ClassToAnalyze @class, IAnalysisContext context)
		{
			var methods = @class.GetMethods(context);
			var properties = @class.GetProperties(context);

			return GenerationSemantics.GetBuilder(@class).Build(_ =>
			{
				var factory = FactorySemantics.GetBuilder(@class)?.Build(_ =>
				{
					var resolvers = AggregateResolvers(methods);
					var collectionResolvers = AggregateCollectionResolvers(methods);

					var delegations = properties.Select(DelegationSemantics.GetBuilder).Build(p =>
					{
						var ms = p.GetMethodToAnalyze();
						var dr = AggregateResolvers(ms);
						var dcr = AggregateCollectionResolvers(ms);
						return (dr, dcr);
					});

					return (resolvers, collectionResolvers, delegations);
				});

				if (factory is null)
				{
					return (new string[0], null, new DependencySemantics[0]);
				}

				var dependencies = DependencySemantics.FromFactory(factory);
				var namespaces = AggregateNamespaces(factory, dependencies).ToArray();

				return (namespaces, factory, dependencies);
			});
		}

		private ResolverSemantics[] AggregateResolvers(MethodToAnalyze[] methods)
		{
			return methods.Select(ResolverSemantics.GetBuilder).Build(m =>
			{
				var attr = m.GetAttributes();

				var ret = m.GetReturnType(_context) is { } t
					? ResolutionSemantics.Build(t, _context)
					: null;
				var (parameters, resolutions, hooks) = LoadMethodFeature(m.GetParameters(), attr, m.Symbol.Name);

				return (ret, resolutions, parameters, hooks);
			});
		}

		private CollectionResolverSemantics[] AggregateCollectionResolvers(MethodToAnalyze[] methods)
		{
			return methods.Select(CollectionResolverSemantics.GetBuilder).Build(
				m => LoadMethodFeature(m.GetParameters(), m.GetAttributes(), m.Symbol.Name));
		}

		private (ParameterSemantics[], ResolutionSemantics[], HookSemantics[]) LoadMethodFeature(
			ParameterToAnalyze[] parameters, AttributeToAnalyze[] attributes, string methodName)
		{
			var ps = Build(parameters, p => ParameterSemantics.Build(p, _context));
			var rs = Build(attributes, a => ResolutionSemantics.Build(a, _context));
			var hs = Build(attributes, a => HookSemantics.Build(a, _context, methodName));
			return (ps, rs, hs);
		}

		private TResult[] Build<T, TResult>(IEnumerable<T> source, Func<T, TResult?> selector)
			where TResult : class
		{
			return source.Select(selector)
				.FilterNull()
				.ToArray();
		}

		private static IEnumerable<string> AggregateNamespaces(FactorySemantics semantics,
			DependencySemantics[] dependencies)
		{
			return semantics.Resolvers.Cast<INamespaceClaimer>()
				.Concat(semantics.CollectionResolvers)
				.Concat(semantics.Delegations)
				.Concat(semantics.Resolvers.SelectMany(x => x.Hooks))
				.Concat(dependencies)
				.SelectMany(x => x.GetRequiredNamespaces())
				.Except(semantics.Type.FullNamespace.WrapByArray())
				.Distinct();
		}
	}
}
