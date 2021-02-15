using System.Linq;
using System.Reactive;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record Parameter(TypeName TypeName, string ParameterName)
	{
		public static Parameter[] FromResolver(IMethodSymbol symbol)
		{
			return symbol.Parameters
				.Select(x => new Parameter(TypeName.FromSymbol(x.Type), x.Name))
				.ToArray();
		}

		public static Builder<Aspects.Nodes.Parameter, Unit, Parameter>? GetBuilder(
			Aspects.Nodes.Parameter parameter, IAnalysisContext context)
		{
			if (parameter.Syntax.Type is null
			    || context.GetTypeSymbol(parameter.Syntax.Type) is not { } symbol)
			{
				return null;
			}

			return new(parameter, unit => new Parameter(
				Utilities.TypeName.FromSymbol(symbol),
				parameter.Syntax.Identifier.ValueText));
		}

		public static Parameter? Build(Aspects.Nodes.Parameter parameter, IAnalysisContext context)
		{
			if (parameter.Syntax.Type is null
				|| context.GetTypeSymbol(parameter.Syntax.Type) is not {} symbol)
			{
				return null;
			}

			return new Parameter(
				TypeName.FromSymbol(symbol),
				parameter.Syntax.Identifier.ValueText);
		}
	}
}
