using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Steps.Aspects.Nodes
{
	internal record ReturnType(TypeSyntax Syntax, INamedTypeSymbol Symbol)
	{
		public bool IsResolution()
		{
			return !Symbol.IsAbstract;
		}
	}
}
