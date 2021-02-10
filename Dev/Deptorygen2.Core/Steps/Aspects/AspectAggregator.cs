using Deptorygen2.Core.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Steps.Aggregation
{
	internal class AspectAggregator
	{
		public ClassToAnalyze? Aggregate(ClassDeclarationSyntax root, IAnalysisContext context)
		{
			return context.GetNamedTypeSymbol(root) is { } symbol
				? new ClassToAnalyze(root, symbol)
				: null;
		}
	}
}
