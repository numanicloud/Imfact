using System;
using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Aggregation;
using Deptorygen2.Core.Interfaces;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Semanticses
{
	internal class SemanticsAggregator
	{
		private readonly IAnalysisContext _context;

		public SemanticsAggregator(IAnalysisContext context)
		{
			_context = context;
		}

		public FactorySemantics? Aggregate(ClassToAnalyze @class, IAnalysisContext context)
		{
			var methods = @class.GetMethods(context);
			var properties = @class.GetProperties(context);

			return FactorySemantics.Build(@class, partial =>
			{
				var (resolvers, collectionResolvers) = LoadResolvers(methods);

				var delegations = Build(properties, p => DelegationSemantics.Build(p, partial1 =>
				{
					var (dr, dcr) = LoadResolvers(methods);
					return partial1.Complete(dr, dcr);
				}));

				return partial.Complete(resolvers, collectionResolvers, delegations);
			});
		}

		private (ResolverSemantics[], CollectionResolverSemantics[]) LoadResolvers(MethodToAnalyze[] methods)
		{
			var resolvers = Build(methods, m => ResolverSemantics.Build(m, partial1 =>
			{
				var ret = m.GetReturnType(_context) is { } t
					? ResolutionSemantics.Build(t)
					: null;
				var (parameters, resolutions) = LoadMethodFeature(m.GetParameters(), m.GetAttributes());

				return partial1.Complete(ret, resolutions, parameters);
			}));

			var collectionResolvers = Build(methods, m => CollectionResolverSemantics.Build(m,
				partial1 =>
				{
					var (parameters, resolutions) = LoadMethodFeature(m.GetParameters(), m.GetAttributes());
					return partial1.Complete(parameters, resolutions);
				}));

			return (resolvers, collectionResolvers);
		}

		private (ParameterSemantics[], ResolutionSemantics[]) LoadMethodFeature(
			ParameterToAnalyze[] parameters, AttributeToAnalyze[] attributes)
		{
			var ps = Build(parameters, p => ParameterSemantics.Build(p, _context));
			var rs = Build(attributes, a => ResolutionSemantics.Build(a, _context));
			return (ps, rs);
		}

		private TResult[] Build<T, TResult>(IEnumerable<T> source, Func<T, TResult?> selector)
			where TResult : class
		{
			return source.Select(selector)
				.FilterNull()
				.ToArray();
		}
	}
}
