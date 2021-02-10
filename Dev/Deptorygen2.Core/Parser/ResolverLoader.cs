using System;
using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Parser
{
	internal record ResolverFact(MethodDeclarationSyntax Syntax,
		Accessibility Accessibility,
		TypeName ReturnType,
		string MethodName,
		ParameterSyntax[] Parameters,
		AttributeListSyntax[] AttributeLists)
	{
		public ResolverBaseSemantics GetSemantics(
			ParameterSemantics[] parameters, ResolutionSemantics[] resolutions)
		{
			return new(this, Accessibility, ReturnType, MethodName, parameters, resolutions);
		}
	}

	internal record ResolverBaseSemantics(ResolverFact Fact,
		Accessibility Accessibility,
		TypeName ReturnType,
		string MethodName,
		ParameterSemantics[] Parameters,
		ResolutionSemantics[] Resolutions)
	{
		public ResolverSemantics GetResolverSemantics(ResolutionSemantics? returnTypeResolution)
		{
			return new(MethodName, ReturnType, returnTypeResolution, Resolutions,
				Parameters, Accessibility);
		}
	}

	internal class ResolverLoader
	{
		public delegate ResolverBaseSemantics Completion(ResolverFact resolver);

		private readonly Predicate<ResolverAnalysisContext> _filter;
		private readonly ResolutionLoader _resolutionLoader;
		private readonly IAnalysisContext _context;

		public ResolverLoader(Predicate<ResolverAnalysisContext> filter,
			ResolutionLoader resolutionLoader,
			IAnalysisContext context)
		{
			/* 仕様都合の条件：
			 *		partialである
			 *		通常の明示的なメソッドである
			 */
			_filter = filter;
			_resolutionLoader = resolutionLoader;
			_context = context;
		}

		public ResolverBaseSemantics? Build(MethodDeclarationSyntax syntax, Completion completion)
		{
			if (GetFact(syntax) is not {} fact)
			{
				return null;
			}

			return completion.Invoke(fact);
		}

		public ResolverFact? GetFact(MethodDeclarationSyntax syntax)
		{
			if (_context.GetMethodSymbol(syntax) is not { } symbol
				|| !syntax.IsPartial()
				|| symbol.MethodKind != MethodKind.Ordinary)
			{
				return null;
			}

			return new ResolverFact(syntax,
				symbol.DeclaredAccessibility,
				TypeName.FromSymbol(symbol.ReturnType),
				symbol.Name,
				syntax.ParameterList.Parameters.ToArray(),
				syntax.AttributeLists.ToArray());
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
	}
}
