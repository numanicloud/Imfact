using Deptorygen2.Core.Interfaces;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record Parameter(TypeNode Type, string ParameterName)
	{
		public static Parameter? Build(Aspects.Nodes.Parameter parameter, IAnalysisContext context)
		{
			if (parameter.Syntax.Type is null
				|| context.GetTypeSymbol(parameter.Syntax.Type) is not {} symbol)
			{
				return null;
			}

			return new Parameter(
				TypeNode.FromSymbol(symbol),
				parameter.Syntax.Identifier.ValueText);
		}
	}
}
