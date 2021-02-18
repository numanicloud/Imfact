using System.Linq;
using Deptorygen2.Annotations;
using Deptorygen2.Core.Interfaces;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record EntryResolver(string MethodName,
		TypeNode ReturnType,
		Parameter[] Parameters,
		Accessibility Accessibility)
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
	}
}
