using Imfact.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Imfact.Steps.Ranking
{
	internal record RankedClass(INamedTypeSymbol Symbol,
		INamedTypeSymbol? BaseSymbol,
		int Rank);
}
