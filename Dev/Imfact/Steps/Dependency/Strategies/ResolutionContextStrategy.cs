using Deptorygen2.Annotations;
using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Steps.Dependency.Components;
using Deptorygen2.Core.Steps.Expressions;

namespace Deptorygen2.Core.Steps.Dependency.Strategies
{
	internal class ResolutionContextStrategy : IExpressionStrategy
	{
		public ICreationNode? GetExpression(CreationContext context)
		{
			if (context.TypeToResolve[0].Record == TypeRecord.FromRuntime(typeof(ResolutionContext)))
			{
				return new Variable("context");
			}

			return null;
		}
	}
}
