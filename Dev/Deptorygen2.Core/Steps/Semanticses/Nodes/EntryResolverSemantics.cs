using System.Linq;
using Deptorygen2.Annotations;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record EntryResolverSemantics(string MethodName,
		TypeName ReturnType,
		ParameterSemantics[] Parameters,
		Accessibility Accessibility)
	{
		private static readonly TypeName CtxType = TypeName.FromType(typeof(ResolutionContext));

		public static EntryResolverSemantics FromResolver(ResolverSemantics resolver)
		{
			return new EntryResolverSemantics(
				resolver.MethodName.TrimStart("__".ToCharArray()),
				resolver.ReturnType,
				resolver.Parameters.Where(x => x.TypeName != CtxType).ToArray(),
				resolver.Accessibility);
		}
	}
}
