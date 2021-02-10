﻿using System;
using System.Linq;
using Deptorygen2.Core.Aggregation;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Semanticses
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
				return BuildInternal(symbol);
			}

			return null;
		}

		public static ResolutionSemantics? Build(ReturnTypeToAnalyze returnType)
		{
			return BuildInternal(returnType.Symbol);
		}

		private static ResolutionSemantics? BuildInternal(INamedTypeSymbol symbol)
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
