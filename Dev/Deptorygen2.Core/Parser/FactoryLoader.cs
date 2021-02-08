using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deptorygen2.Core.Structure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Syntaxes.Parser
{
	internal delegate void FactoryContentsLoader(FactoryAnalysisContext factory,
		List<ResolverSyntax> singles,
		List<CollectionResolverSyntax> collections,
		List<DelegationSyntax> delegations);

	internal class FactoryLoader
	{
		public async Task<FactorySyntax> BuildFactorySyntaxAsync(
			ClassDeclarationSyntax classDeclarationSyntax,
			SourceGenAnalysisContext context,
			FactoryContentsLoader loadContents)
		{
			var factory = await GetContextAsync(classDeclarationSyntax, context);
			var singles = new List<ResolverSyntax>();
			var collections = new List<CollectionResolverSyntax>();
			var delegations = new List<DelegationSyntax>();

			loadContents.Invoke(factory, singles, collections, delegations);

			return new FactorySyntax(factory.Symbol, singles.ToArray(), collections.ToArray(), delegations.ToArray());
		}

		public async Task<FactoryAnalysisContext> GetContextAsync(
			ClassDeclarationSyntax syntax,
			SourceGenAnalysisContext context)
		{
			var symbol = await GetSymbolOf(syntax, context);
			return new FactoryAnalysisContext(syntax, symbol, context);
		}

		private async Task<INamedTypeSymbol> GetSymbolOf(
			TypeDeclarationSyntax syntax,
			SourceGenAnalysisContext context)
		{
			var symbols = await context.FindSourceDeclarationSymbolAsync(syntax);

			var @namespace = syntax.Parent is not NamespaceDeclarationSyntax nsds ? throw new Exception()
				: nsds.Name is not QualifiedNameSyntax qns ? throw new Exception()
				: qns.ToString();

			return symbols.OfType<INamedTypeSymbol>().First(x => x.GetFullNameSpace() == @namespace);
		}
	}
}
