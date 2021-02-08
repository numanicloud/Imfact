using System;
using System.Linq;
using System.Threading.Tasks;
using Deptorygen2.Core.Structure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Syntaxes.Parser
{
	internal class FactoryLoader
	{
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
