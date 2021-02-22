using Deptorygen2.Core.Steps.Creation;

namespace Deptorygen2.Core.Steps.Expressions
{
	internal interface IExpressionStrategy
	{
		ICreationNode? GetExpression(CreationContext context);
	}
}
