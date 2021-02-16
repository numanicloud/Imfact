using System.Collections.Generic;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record Parameter(TypeNode Type, string ParameterName)
		: INamespaceClaimer
	{
		public static Parameter? Build(Aspects.Nodes.Parameter parameter, IAnalysisContext context)
		{
			if (parameter.Syntax.Type is null
				|| context.GetTypeSymbol(parameter.Syntax.Type) is not {} symbol)
			{
				return null;
			}

			return new Parameter(
				TypeNode.FromSymbol(symbol),
				parameter.Syntax.Identifier.ValueText);
		}

		public IEnumerable<ISemanticsNode> Traverse()
		{
			yield return Type;
		}

		public IEnumerable<string> GetRequiredNamespaces()
		{
			yield return Type.FullNamespace;
		}
	}
}
