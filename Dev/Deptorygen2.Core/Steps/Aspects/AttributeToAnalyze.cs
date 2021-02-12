using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Steps.Aggregation
{
	internal record AttributeToAnalyze(AttributeData Data)
	{
		public bool IsResolution()
		{
			var name = new AttributeName("ResolutionAttribute");
			return name.MatchWithAnyName(Data.AttributeClass?.Name ?? "")
			       && Data.ConstructorArguments.Length == 1
			       && Data.ConstructorArguments[0].Kind == TypedConstantKind.Type;
		}

		public bool IsHook()
		{
			var name = new AttributeName("HookAttribute");
			return name.MatchWithAnyName(Data.AttributeClass?.Name ?? "")
			       && Data.ConstructorArguments.Length == 1
			       && Data.ConstructorArguments[0].Kind == TypedConstantKind.Type;
		}
	}
}