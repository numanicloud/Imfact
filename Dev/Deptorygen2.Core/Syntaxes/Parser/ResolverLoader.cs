using System;
using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Structure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Syntaxes.Parser
{
	internal class ResolverLoader
	{
		private readonly Predicate<ResolverStructure> _filter;

		public ResolverLoader(Predicate<ResolverStructure> filter)
		{
			/* 仕様都合の条件：
			 *		partialである
			 *		通常の明示的なメソッドである
			 */
			_filter = filter;
		}

		public IEnumerable<ResolverStructure> ExtractResolverMethods(FactoryAnalysisContext container)
		{
			return ResolverStructures(container, container.Symbol, container.Symbol.BaseType);
		}

		private IEnumerable<ResolverStructure> ResolverStructures(
			FactoryAnalysisContext container, params INamedTypeSymbol?[] holders)
		{
			var structures = from members in
					from holder in holders.FilterNull()
					select holder.GetMembers()
				from member in members.OfType<IMethodSymbol>()
				select GetStructure(member, container);

			return from structure in structures.FilterNull()
				where _filter(structure)
				select structure;
		}

		private ResolverStructure? GetStructure(IMethodSymbol symbol, FactoryAnalysisContext factoryContext)
		{
			return MethodDeclarationSyntax(symbol) is { } syntax
				? new ResolverStructure(syntax, symbol, factoryContext)
				: null;
		}

		private MethodDeclarationSyntax? MethodDeclarationSyntax(IMethodSymbol symbol)
		{
			return symbol.DeclaringSyntaxReferences
				.Select(x => x.GetSyntax())
				.OfType<MethodDeclarationSyntax>()
				.FilterNull()
				.FirstOrDefault();
		}
	}
}
