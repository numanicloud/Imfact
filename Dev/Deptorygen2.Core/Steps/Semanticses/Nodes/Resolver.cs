using Deptorygen2.Core.Interfaces;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record Resolver(ResolverCommon Common, Resolution? ReturnTypeResolution)
		: IResolverSemantics
	{
		public TypeNode ReturnType => Common.ReturnType;
		public string MethodName => Common.MethodName;
		public Parameter[] Parameters => Common.Parameters;
		public Accessibility Accessibility => Common.Accessibility;
		public Resolution[] Resolutions => Common.Resolutions;
		public Hook[] Hooks => Common.Hooks;

		public IEnumerable<ISemanticsNode> Traverse()
		{
			yield return Common;
			if (ReturnTypeResolution is not null)
			{
				yield return ReturnTypeResolution;
			}
		}
	}
}
