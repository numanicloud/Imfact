using Deptorygen2.Core.Interfaces;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record ResolverCommon(
		Accessibility Accessibility,
		TypeNode ReturnType,
		string MethodName,
		Parameter[] Parameters,
		Resolution[] Resolutions,
		Hook[] Hooks) : IResolverSemantics
	{
	}
}
