using Imfact.Steps.Dependency.Components;

namespace Imfact.Steps.Dependency.Interfaces
{
	internal interface IExpressionStrategy
	{
		ICreationNode? GetExpression(CreationContext context);
	}
}
