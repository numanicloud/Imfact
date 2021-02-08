using System.Linq;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Syntaxes
{
	internal record ParameterSyntax(TypeName TypeName, string ParameterName)
	{
		public static ParameterSyntax[] FromResolver(IMethodSymbol symbol)
		{
			return symbol.Parameters
				.Select(x => new ParameterSyntax(TypeName.FromSymbol(x.Type), x.Name))
				.ToArray();
		}
	}
}
