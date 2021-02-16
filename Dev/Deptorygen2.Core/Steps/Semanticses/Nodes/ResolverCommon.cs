using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record ResolverCommon(
		Accessibility Accessibility,
		TypeName ReturnType,
		string MethodName,
		Parameter[] Parameters,
		Resolution[] Resolutions,
		Hook[] Hooks) : IResolverSemantics
	{
	}
}
