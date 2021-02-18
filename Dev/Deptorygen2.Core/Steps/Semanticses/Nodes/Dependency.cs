using System.Collections.Generic;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record Dependency(TypeNode TypeName, string FieldName) : ISemanticsNode
	{
		public IEnumerable<ISemanticsNode> Traverse()
		{
			yield return this;
		}
	}
}
