using System.Diagnostics.SymbolStore;
using System.Linq;
using Deptorygen2.Core.Aggregation;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Semanticses
{
	internal record ParameterSemantics(TypeName TypeName, string ParameterName)
	{
		public static ParameterSemantics[] FromResolver(IMethodSymbol symbol)
		{
			return symbol.Parameters
				.Select(x => new ParameterSemantics(TypeName.FromSymbol(x.Type), x.Name))
				.ToArray();
		}

		public static ParameterSemantics? Build(ParameterToAnalyze parameter, IAnalysisContext context)
		{
			if (parameter.Syntax.Type is null
				|| context.GeTypeSymbol(parameter.Syntax.Type) is not {} symbol)
			{
				return null;
			}

			return new ParameterSemantics(
				TypeName.FromSymbol(symbol),
				parameter.Syntax.Identifier.ValueText);
		}
	}
}
