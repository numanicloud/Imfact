using Deptorygen2.Core.Steps.Dependency.Components;

namespace Deptorygen2.Core.Steps.Expressions
{
	internal interface IExpressionStrategy
	{
		ICreationNode? GetExpression(CreationContext context);
	}
}
