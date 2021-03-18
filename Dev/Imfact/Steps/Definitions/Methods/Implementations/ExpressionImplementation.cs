using Imfact.Entities;
using Imfact.Steps.Definitions.Interfaces;
using Imfact.Utilities;

namespace Imfact.Steps.Definitions.Methods
{
	internal record ExpressionImplementation(Hook[] Hooks, TypeAnalysis ReturnType, string Expression) : Implementation
	{
		public override void Render(IFluentCodeBuilder builder, IResolverWriter writer)
		{
			writer.Render(ReturnType.GetCode(), Hooks, Expression, builder);
		}
	}
}
