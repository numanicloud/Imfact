using System.Collections.Generic;

namespace Deptorygen2.Core.Steps.Semanticses.Interfaces
{
	internal interface ISemanticsNode
	{
		IEnumerable<ISemanticsNode> Traverse();
	}
}
