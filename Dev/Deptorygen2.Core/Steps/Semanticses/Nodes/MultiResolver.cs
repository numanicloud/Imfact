using Deptorygen2.Core.Interfaces;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record MultiResolver(ResolverCommon Common)
		: IResolverSemantics
	{
		public TypeNode ReturnType => Common.ReturnType;
		public string MethodName => Common.MethodName;
		public Parameter[] Parameters => Common.Parameters;
		public Accessibility Accessibility => Common.Accessibility;
		public Resolution[] Resolutions => Common.Resolutions;
		public Hook[] Hooks => Common.Hooks;

		public TypeNode ElementType => ReturnType.TypeArguments[0];
	}
}
