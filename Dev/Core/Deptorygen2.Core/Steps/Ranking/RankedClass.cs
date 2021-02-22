using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Steps.Ranking
{
	internal record RankedClass(ClassDeclarationSyntax Syntax,
		INamedTypeSymbol Symbol,
		INamedTypeSymbol? BaseSymbol,
		int Rank);
}
