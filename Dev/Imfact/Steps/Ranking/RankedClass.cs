using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Ranking
{
	internal record RankedClass(INamedTypeSymbol Symbol,
		INamedTypeSymbol? BaseSymbol,
		int Rank);
}
