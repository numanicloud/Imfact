using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Expressions
{
	internal class ExpressionBuilder
	{
		private readonly SemanticsRoot _semantics;
		private readonly CreationCrawler _crawler;

		public ExpressionBuilder(SemanticsRoot semantics)
		{
			_semantics = semantics;
			_crawler = new CreationCrawler(semantics);
		}

		public ResolutionRoot Build()
		{
			Dictionary<IResolverSemantics, CreationExpTree> result = new();
			Dictionary<IResolverSemantics, MultiCreationExpTree> resultMulti = new();

			var methods = _semantics.Factory.Resolvers;
			foreach (var method in methods)
			{
				var context = new CreationContext(method,
					method.ActualResolution.TypeName.WrapByArray(),
					new List<Parameter>(),
					_crawler);
				var creation = _crawler.GetExpression(context).First();
				result[method] = new CreationExpTree(creation);
			}

			var multi = _semantics.Factory.MultiResolvers;
			foreach (var multiResolver in multi)
			{
				var context = new CreationContext(multiResolver,
					multiResolver.Resolutions.Select(x => x.TypeName).ToArray(),
					new List<Parameter>(),
					_crawler);
				var creations = _crawler.GetExpression(context);
				resultMulti[multiResolver] = new MultiCreationExpTree(creations.ToArray());
			}

			var deps = result.Values.SelectMany(x => FindFields(x.Root))
				.Concat(resultMulti.Values.SelectMany(x => x.Roots.SelectMany(FindFields)))
				.GroupBy(x => x.Type.Record)
				.Select(x => x.First())
				.Select(x => new Dependency(x.Type, x.Name))
				.ToArray();

			var injectionResult = new InjectionResult(result, resultMulti, deps);

			var usingRule = new UsingRule();
			var usings = usingRule.Extract(_semantics, injectionResult);

			return new ResolutionRoot(_semantics, injectionResult, usings);
		}

		private IEnumerable<UnsatisfiedField> FindFields(ICreationNode pivot)
		{
			if (pivot is UnsatisfiedField field)
			{
				yield return field;
			}
			else if (pivot is Invocation invocation)
			{
				foreach (var arg in invocation.Arguments)
				{
					foreach (var child in FindFields(arg))
					{
						yield return child;
					}
				}
			}
		}
	}
}
