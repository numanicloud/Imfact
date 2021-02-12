using System.Linq;
using System.Reactive;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Aggregation;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Steps.Semanticses
{
	internal record ParameterSemantics(TypeName TypeName, string ParameterName)
	{
		public static ParameterSemantics[] FromResolver(IMethodSymbol symbol)
		{
			return symbol.Parameters
				.Select(x => new ParameterSemantics(TypeName.FromSymbol(x.Type), x.Name))
				.ToArray();
		}

		public static Builder<ParameterToAnalyze, Unit, ParameterSemantics>? GetBuilder(
			ParameterToAnalyze parameter, IAnalysisContext context)
		{
			if (parameter.Syntax.Type is null
			    || context.GetTypeSymbol(parameter.Syntax.Type) is not { } symbol)
			{
				return null;
			}

			return new(parameter, unit => new ParameterSemantics(
				Utilities.TypeName.FromSymbol(symbol),
				parameter.Syntax.Identifier.ValueText));
		}

		public static ParameterSemantics? Build(ParameterToAnalyze parameter, IAnalysisContext context)
		{
			if (parameter.Syntax.Type is null
				|| context.GetTypeSymbol(parameter.Syntax.Type) is not {} symbol)
			{
				return null;
			}

			return new ParameterSemantics(
				TypeName.FromSymbol(symbol),
				parameter.Syntax.Identifier.ValueText);
		}
	}
}
