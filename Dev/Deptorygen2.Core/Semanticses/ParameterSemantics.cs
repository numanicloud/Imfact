using System.Linq;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Semanticses
{
	internal record ParameterSemantics(TypeName TypeName, string ParameterName)
	{
		public static ParameterSemantics[] FromResolver(IMethodSymbol symbol)
		{
			return symbol.Parameters
				.Select(x => new ParameterSemantics(TypeName.FromSymbol(x.Type), x.Name))
				.ToArray();
		}
	}
}
