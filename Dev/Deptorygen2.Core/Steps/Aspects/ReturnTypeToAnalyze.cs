using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Steps.Aggregation
{
	internal record ReturnTypeToAnalyze(TypeSyntax Syntax, INamedTypeSymbol Symbol)
	{
	}
}
