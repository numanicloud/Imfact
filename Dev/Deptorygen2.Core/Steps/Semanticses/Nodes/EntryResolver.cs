using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Annotations;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record EntryResolver(string MethodName,
		TypeNode ReturnType,
		Parameter[] Parameters,
		Accessibility Accessibility) : ISemanticsNode
	{
		private static readonly TypeNode CtxType = TypeNode.FromRuntime(typeof(ResolutionContext));

		public static EntryResolver FromResolver(IResolverSemantics resolver)
		{
			return new EntryResolver(
				resolver.MethodName.TrimStart("__".ToCharArray()),
				resolver.ReturnType,
				resolver.Parameters.Where(x => !x.Type.Record.Equals(CtxType.Record)).ToArray(),
				resolver.Accessibility);
		}

		public IEnumerable<ISemanticsNode> Traverse()
		{
			foreach (var parameter in Parameters)
			{
				yield return parameter;
			}
		}
	}
}
