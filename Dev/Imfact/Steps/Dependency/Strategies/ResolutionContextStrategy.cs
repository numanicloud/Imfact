using Imfact.Annotations;
using Imfact.Entities;
using Imfact.Steps.Dependency.Components;
using Imfact.Steps.Dependency.Interfaces;

namespace Imfact.Steps.Dependency.Strategies
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
