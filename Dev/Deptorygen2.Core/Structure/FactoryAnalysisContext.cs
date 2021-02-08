using System.Collections.Generic;
using System.Linq;
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

		public async Task<IEnumerable<ISymbol>> FindSourceDeclarationSymbolAsync(
			string symbolName)
		{
			return await SymbolFinder.FindSourceDeclarationsAsync(
				Document.Project,
				symbolName,
				false,
				Token);
		}

		public async Task<IEnumerable<ISymbol>> FindSourceDeclarationSymbolAsync(
			BaseTypeSyntax syntax)
		{
			var id = syntax.ChildNodes().OfType<IdentifierNameSyntax>()
				.FirstOrDefault();

			if (id is null)
			{
				return Enumerable.Empty<ISymbol>();
			}

			return await SymbolFinder.FindSourceDeclarationsAsync(
				Document.Project,
				id.Identifier.ValueText,
				false,
				Token);
		}
	}

	internal record FactoryAnalysisContext(
		ClassDeclarationSyntax Syntax,
		INamedTypeSymbol Symbol,
		SourceGenAnalysisContext Context);

	internal record ResolverStructure(
		MethodDeclarationSyntax Syntax,
		IMethodSymbol Symbol,
		INamedTypeSymbol Return,
		ResolutionStructure[] Resolutions,
		FactoryAnalysisContext Factory);

	internal record CollectionResolverStructure(
		MethodDeclarationSyntax Syntax,
		IMethodSymbol Symbol,
		ResolutionStructure[] Resolutions,
		FactoryAnalysisContext Factory);

	internal record ResolutionStructure(INamedTypeSymbol Symbol);
}
