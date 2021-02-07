using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Deptorygen.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using IServiceProvider = Deptorygen.Generator.Interfaces.IServiceProvider;

namespace Deptorygen.Generator.Syntaxes
{
	class FactorySyntax : IServiceProvider
	{
		public INamedTypeSymbol InterfaceSymbol { get; }
		public ResolverSyntax[] Resolvers { get; }
		public CollectionResolverSyntax[] CollectionResolvers { get; }
		public CaptureSyntax[] Captures { get; }

		public FactorySyntax(INamedTypeSymbol interfaceSymbol,
			ResolverSyntax[] resolvers,
			CollectionResolverSyntax[] collectionResolvers,
			CaptureSyntax[] captures)
		{
			InterfaceSymbol = interfaceSymbol;
			Resolvers = resolvers;
			CollectionResolvers = collectionResolvers;
			Captures = captures;
		}

		public static async Task<FactorySyntax> FromDeclarationAsync(InterfaceDeclarationSyntax syntax, Document document, CancellationToken ct)
		{
			async Task<INamedTypeSymbol> GetSymbol()
			{
				var ns = syntax.Parent is NamespaceDeclarationSyntax nsds
					? (nsds.Name is QualifiedNameSyntax qns ? qns.ToString() : throw new Exception())
					: throw new Exception();

				var symbols = await SymbolFinder.FindSourceDeclarationsAsync(
					document.Project,
					syntax.Identifier.ValueText,
					false,
					ct);

				var namedTypeSymbol = symbols.OfType<INamedTypeSymbol>()
					.First(x => x.GetFullNameSpace() == ns);
				return namedTypeSymbol;
			}

			var symbol = await GetSymbol();
			var resolvers = ResolverSyntax.FromParent(symbol);
			var captures = CaptureSyntax.FromFactory(symbol);

			return new FactorySyntax(symbol, resolvers.Item1, resolvers.Item2, captures);
		}

		public IEnumerable<TypeName> GetCapableServiceTypes()
		{
			yield return TypeName.FromSymbol(InterfaceSymbol);
			foreach (var capture in Captures)
			{
				yield return capture.TypeName;
			}
		}
	}
}
