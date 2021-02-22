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

			var methods = _semantics.Factory.Resolvers;
			foreach (var method in methods)
			{
				var context = new CreationContext(method,
					method.ActualResolution.TypeName.WrapByArray(),
					method.Parameters.ToList(),
					_crawler);
				var creation = _crawler.GetExpression(context).First();
				result[method] = new CreationExpTree(creation);
			}

			var deps = result.Values.SelectMany(x => FindFields(x.Root))
				.Select(x => new Dependency(x.Type, x.Name))
				.ToArray();

			return new ResolutionRoot(_semantics, new InjectionResult(result, deps));
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
