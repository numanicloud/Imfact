using System.Collections.Generic;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
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
		Hook[] Hooks) : IResolverSemantics, ISemanticsNode
	{
		public IEnumerable<ISemanticsNode> Traverse()
		{
			yield return this;

			foreach (var parameter in Parameters)
			{
				yield return parameter;
			}

			foreach (var resolution in Resolutions)
			{
				yield return resolution;
			}

			foreach (var hook in Hooks)
			{
				yield return hook;
			}
		}
	}
}
