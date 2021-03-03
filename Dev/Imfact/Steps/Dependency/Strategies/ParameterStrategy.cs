using System.Linq;
using Imfact.Steps.Dependency.Components;
using Imfact.Steps.Dependency.Interfaces;

namespace Imfact.Steps.Dependency.Strategies
{
	class ParameterStrategy : IExpressionStrategy
	{
		public ICreationNode? GetExpression(CreationContext context)
		{
			var ttr = context.TypeToResolve[0].Record;
			var resolution = context.Caller.Parameters
				.Except(context.ConsumedParameters)
				.FirstOrDefault(x => x.Type.Record == ttr);

			if (resolution is null)
			{
				return null;
			}

			context.ConsumedParameters.Add(resolution);
			return new Variable(resolution.ParameterName);
		}
	}
}
