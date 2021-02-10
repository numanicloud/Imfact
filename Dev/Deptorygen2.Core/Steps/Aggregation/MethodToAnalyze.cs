using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Aggregation
{
	internal record MethodToAnalyze(MethodDeclarationSyntax Syntax, IMethodSymbol Symbol)
	{
		public bool IsSingleResolver()
		{
			return !TypeName.FromSymbol(Symbol.ReturnType).IsCollectionType();
		}

		public bool IsCollectionResolver()
		{
			return TypeName.FromSymbol(Symbol.ReturnType).IsCollectionType();
		}

		public ReturnTypeToAnalyze? GetReturnType(IAnalysisContext context)
		{
			if (context.GeTypeSymbol(Syntax.ReturnType) is INamedTypeSymbol symbol)
			{
				return new(Syntax.ReturnType, symbol);
			}

			return null;
		}

		public AttributeToAnalyze[] GetAttributes()
		{
			return Syntax.AttributeLists.SelectMany(x => x.Attributes)
				.Select(x => new AttributeToAnalyze(x))
				.ToArray();
		}

		public ParameterToAnalyze[] GetParameters()
		{
			return Syntax.ParameterList.Parameters
				.Select(x => new ParameterToAnalyze(x))
				.ToArray();
		}
	}
}
