using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Steps.Semanticses.Nodes;

namespace Deptorygen2.Core.Steps.Expressions.Strategies
{
	class ConstructorStrategy : IExpressionStrategy
	{
		public ICreationNode? GetExpression(CreationContext context)
		{
			var ttr = context.TypeToResolve[0].Record;

			var resolution = context.Caller.Resolutions
				.FirstOrDefault(x => x.TypeName.Record == ttr);
			if (resolution is not null)
			{
				return new Invocation($"new {ttr.Name}", GetArgs(resolution, context).ToArray());
			}

			return null;
		}

		private IEnumerable<ICreationNode> GetArgs(Resolution resolution, CreationContext context)
		{
			var childContext = context with
			{
				TypeToResolve = resolution.Dependencies
			};
			return context.Injector.GetExpression(childContext);
		}
	}
}
