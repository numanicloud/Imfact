using System;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Aggregation;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Steps.Semanticses
{
	internal record ResolutionSemantics(TypeName TypeName,
		TypeName[] Dependencies,
		bool IsDisposable)
	{
		public static ResolutionSemantics? Build(AttributeToAnalyze attribute,
			IAnalysisContext context)
		{
			if (!attribute.IsResolution())
			{
				return null;
			}

			var argument1 = attribute.Syntax.ArgumentList?.Arguments[0];

			if (argument1?.Expression is TypeOfExpressionSyntax toes
				&& context.GeTypeSymbol(toes.Type) is INamedTypeSymbol symbol)
			{
				return BuildInternal(toes.Type, symbol, context);
			}

			return null;
		}

		public static ResolutionSemantics? Build(ReturnTypeToAnalyze returnType, IAnalysisContext context)
		{
			if (!returnType.IsResolution())
			{
				return null;
			}

			return BuildInternal(returnType.Syntax, returnType.Symbol, context);
		}

		private static ResolutionSemantics? BuildInternal(TypeSyntax syntax,
			INamedTypeSymbol symbol,
			IAnalysisContext context)
		{
			var isDisposable = symbol.IsImplementing(typeof(IDisposable));

			var dependencies = symbol.Constructors.Single().Parameters
				.Select(x => TypeName.FromSymbol(x.Type))
				.ToArray();

			return new ResolutionSemantics(
				TypeName.FromSymbol(symbol),
				dependencies,
				isDisposable);
		}
	}
}
