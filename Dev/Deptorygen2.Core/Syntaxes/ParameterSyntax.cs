using System.Linq;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Syntaxes
{
	class ParameterSyntax
	{
		public TypeName TypeName { get; }
		public string ParameterName { get; }

		public ParameterSyntax(TypeName typeName, string parameterName)
		{
			TypeName = typeName;
			ParameterName = parameterName;
		}

		public static ParameterSyntax[] FromResolver(IMethodSymbol symbol)
		{
			return symbol.Parameters
				.Select(x => new ParameterSyntax(TypeName.FromSymbol(x.Type), x.Name))
				.ToArray();
		}
	}
}
