using Imfact.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Imfact
{
	internal record CandidateClass(INamedTypeSymbol Symbol);
}
