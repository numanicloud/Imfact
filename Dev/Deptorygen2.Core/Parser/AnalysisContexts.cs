using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Deptorygen2.Core.Structure
{
	internal record SourceGenAnalysisContext(Document Document, CancellationToken Token)
	{
		public async Task<IEnumerable<ISymbol>> FindSourceDeclarationSymbolAsync(
			BaseTypeDeclarationSyntax syntax)
		{
			return await SymbolFinder.FindSourceDeclarationsAsync(
				Document.Project,
				syntax.Identifier.ValueText,
				false,
				Token);
		}
	}

	internal record FactoryAnalysisContext(
		ClassDeclarationSyntax Syntax,
		INamedTypeSymbol Symbol,
		SourceGenAnalysisContext Context);

	internal record ResolverAnalysisContext(
		MethodDeclarationSyntax Syntax,
		IMethodSymbol Symbol,
		INamedTypeSymbol Return,
		ResolutionAnalysisContext[] Resolutions,
		FactoryAnalysisContext Factory);

	internal record ResolutionAnalysisContext(INamedTypeSymbol Symbol);
}
