using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Aggregation
{
	internal record AttributeToAnalyze(AttributeSyntax Syntax)
	{
		public bool IsResolution()
		{
			var name = new AttributeName("ResolutionAttribute");
			return name.MatchWithAnyName(Syntax.Name.ToString())
			       && Syntax.ArgumentList?.Arguments.Count == 1
			       && Syntax.ArgumentList?.Arguments[0].Expression is TypeOfExpressionSyntax;
		}
	}
}
