using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Aspects.Nodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Steps.Aspects
{
	internal class AspectAggregator
	{
		public Class? Aggregate(ClassDeclarationSyntax root, IAnalysisContext context)
		{
			return context.GetNamedTypeSymbol(root) is { } symbol
				? new Class(root, symbol)
				: null;
		}
	}
}
