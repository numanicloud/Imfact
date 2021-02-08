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
		private readonly Predicate<ResolverAnalysisContext> _filter;
		private readonly ResolutionLoader _resolutionLoader;

		public ResolverLoader(Predicate<ResolverAnalysisContext> filter, ResolutionLoader resolutionLoader)
		{
			/* 仕様都合の条件：
			 *		partialである
			 *		通常の明示的なメソッドである
			 */
			_filter = filter;
			_resolutionLoader = resolutionLoader;
		}

		public IEnumerable<ResolverAnalysisContext> BuildResolverStructures(
			FactoryAnalysisContext container,
			Action<IMethodSymbol, List<ResolutionAnalysisContext>> isolation)
		{
			var methods = Extract(container.Symbol, container.Symbol.BaseType);

			foreach (var methodSymbol in methods)
			{
				var resolutionList = new List<ResolutionAnalysisContext>();

				isolation(methodSymbol, resolutionList);

				var structure = GetStructure(methodSymbol, resolutionList.ToArray(), container);
				if (structure is not null)
				{
					yield return structure;
				}
			}
		}

		private IEnumerable<IMethodSymbol> Extract(params INamedTypeSymbol?[] holders)
		{
			return from members in
					from holder in holders.FilterNull()
					select holder.GetMembers()
				from member in members.OfType<IMethodSymbol>()
				where member.ReturnType is INamedTypeSymbol
				select member;
		}

		private ResolverAnalysisContext? GetStructure(
			IMethodSymbol symbol,
			ResolutionAnalysisContext[] resolutions,
			FactoryAnalysisContext factoryContext)
		{
			return MethodDeclarationSyntax(symbol) is not { } syntax ? null
				: symbol.ReturnType is not INamedTypeSymbol nts ? null
				: new ResolverAnalysisContext(syntax, symbol, nts, resolutions, factoryContext);
		}

		private MethodDeclarationSyntax? MethodDeclarationSyntax(IMethodSymbol symbol)
		{
			return symbol.DeclaringSyntaxReferences
				.Select(x => x.GetSyntax())
				.OfType<MethodDeclarationSyntax>()
				.FilterNull()
				.FirstOrDefault();
		}

		public IEnumerable<ResolverAnalysisContext> ExtractResolverMethods(FactoryAnalysisContext container)
		{
			return ResolverStructures(container, container.Symbol, container.Symbol.BaseType);
		}

		private IEnumerable<ResolverAnalysisContext> ResolverStructures(
			FactoryAnalysisContext container, params INamedTypeSymbol?[] holders)
		{
			var structures = from members in
								 from holder in holders.FilterNull()
								 select holder.GetMembers()
							 from member in members.OfType<IMethodSymbol>()
							 where member.ReturnType is INamedTypeSymbol
							 select GetStructure(member, container);

			return from structure in structures.FilterNull()
				   where _filter(structure)
				   select structure;
		}

		private ResolverAnalysisContext? GetStructure(IMethodSymbol symbol, FactoryAnalysisContext factoryContext)
		{
			var resolutions = _resolutionLoader.GetStructures(symbol);
			return GetStructure(symbol, resolutions, factoryContext);
		}
	}
}
