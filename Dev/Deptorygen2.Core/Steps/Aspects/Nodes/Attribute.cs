using Deptorygen2.Annotations;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Steps.Aspects.Nodes
{
	internal record Attribute(AttributeData Data)
	{
		public bool IsResolution()
		{
			var name = new AttributeName(nameof(ResolutionAttribute));
			return name.MatchWithAnyName(Data.AttributeClass?.Name ?? "")
			       && Data.ConstructorArguments.Length == 1
			       && Data.ConstructorArguments[0].Kind == TypedConstantKind.Type;
		}

		public bool IsHook()
		{
			var name = new AttributeName(nameof(HookAttribute));
			return name.MatchWithAnyName(Data.AttributeClass?.Name ?? "")
			       && Data.ConstructorArguments.Length == 1
			       && Data.ConstructorArguments[0].Kind == TypedConstantKind.Type;
		}

		public bool IsCacheHook()
		{
			var name1 = new AttributeName(nameof(CacheAttribute));
			return name1.MatchWithAnyName(Data.AttributeClass?.Name ?? "");
		}

		public bool IsCachePerResolutionHook()
		{
			var name2 = new AttributeName(nameof(CachePerResolutionAttribute));
			return name2.MatchWithAnyName(Data.AttributeClass?.Name ?? "");
		}
	}
}