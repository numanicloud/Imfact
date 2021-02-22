using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Expressions
{
	internal class ExpressionBuilder
	{
		private readonly Generation _semantics;
		private readonly CreationCrawler _crawler;

		public ExpressionBuilder(Generation semantics)
		{
			_semantics = semantics;
			_crawler = new CreationCrawler(semantics);
		}

		public Dictionary<IResolverSemantics, CreationExpTree> Build()
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

			return result;
		}
	}
}
