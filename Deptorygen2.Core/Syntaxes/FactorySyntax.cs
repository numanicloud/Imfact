using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Deptorygen2.Core.Structure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Deptorygen2.Core.Syntaxes
{
	class FactorySyntax : Interfaces.IServiceProvider
	{
		public INamedTypeSymbol ItselfSymbol { get; }
		public ResolverSyntax[] Resolvers { get; }
		public CollectionResolverSyntax[] CollectionResolvers { get; }
		public DelegationSyntax[] Delegations { get; }

		public FactorySyntax(INamedTypeSymbol itselfSymbol,
			ResolverSyntax[] resolvers,
			CollectionResolverSyntax[] collectionResolvers,
			DelegationSyntax[] delegations)
		{
			ItselfSymbol = itselfSymbol;
			Resolvers = resolvers;
			CollectionResolvers = collectionResolvers;
			Delegations = delegations;
		}

		public static async Task<FactorySyntax> FromDeclarationAsync(
			ClassDeclarationSyntax syntax,
			SourceGenAnalysisContext context)
		{
			var symbol = await GetSymbolOf(syntax, context);
			var structure = new FactoryAnalysisContext(syntax, symbol, context);

			var resolvers = ResolverSyntax.FromParent(structure);
			var delegations = DelegationSyntax.FromFactory(symbol).ToArray();

			return new FactorySyntax(symbol, resolvers.Item1, resolvers.Item2, delegations);
		}

		private static async Task<INamedTypeSymbol> GetSymbolOf(
			TypeDeclarationSyntax syntax,
			SourceGenAnalysisContext context)
		{
			var symbols = await context.FindSourceDeclarationSymbolAsync(syntax);

			var @namespace = syntax.Parent is not NamespaceDeclarationSyntax nsds ? throw new Exception()
				: nsds.Name is not QualifiedNameSyntax qns ? throw new Exception()
				: qns.ToString();

			return symbols.OfType<INamedTypeSymbol>().First(x => x.GetFullNameSpace() == @namespace);
		}

		public IEnumerable<TypeName> GetCapableServiceTypes()
		{
			yield return TypeName.FromSymbol(ItselfSymbol);
			foreach (var delegation in Delegations)
			{
				yield return delegation.TypeName;
			}
		}
	}
}
