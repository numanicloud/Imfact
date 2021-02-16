using System;
using System.Collections.Generic;
using System.Text;

namespace Deptorygen2.Core.Steps.Semanticses.Interfaces
{
	internal interface ISemanticsNode
	{
		IEnumerable<ISemanticsNode> Traverse();
	}
}
