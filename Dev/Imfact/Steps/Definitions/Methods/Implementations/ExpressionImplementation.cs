using Imfact.Entities;
using Imfact.Interfaces;
using Imfact.Steps.Writing;
using Imfact.Steps.Writing.Coding;

namespace Imfact.Steps.Definitions.Methods
{
	internal record ExpressionImplementation(Hook[] Hooks, TypeAnalysis ReturnType, string Expression) : Implementation
	{
		public override void Render(ICodeBuilder builder, IResolverWriter writer)
		{
			writer.Render(ReturnType.GetCode(), Hooks, Expression, builder);
		}
	}
}
