using System.Collections.Generic;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;

namespace Deptorygen2.Core.Interfaces
{
	internal interface INamespaceClaimer : ISemanticsNode
	{
		IEnumerable<string> GetRequiredNamespaces();
	}
}