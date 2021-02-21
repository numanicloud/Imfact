using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Steps.Ranking
{
	public record RankedClass(ClassDeclarationSyntax Syntax,
		INamedTypeSymbol Symbol,
		INamedTypeSymbol? BaseSymbol,
		int Rank);
}
