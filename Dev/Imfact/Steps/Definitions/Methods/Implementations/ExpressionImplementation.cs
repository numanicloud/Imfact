using Imfact.Entities;
using Imfact.Steps.Definitions.Interfaces;
using Imfact.Utilities;
using Imfact.Utilities.Coding;

namespace Imfact.Steps.Definitions.Methods.Implementations;

internal record ExpressionImplementation(Hook[] Hooks, TypeAnalysis ReturnType, string Expression) : Implementation
{
	public override void Render(IFluentCodeBuilder builder, IResolverWriter writer)
	{
		writer.Render(ReturnType.GetCode(), Hooks, Expression, builder);
	}
}