using System.Collections.Generic;

namespace Deptorygen2.Core.Interfaces
{
	internal interface INamespaceClaimer
	{
		IEnumerable<string> GetRequiredNamespaces();
	}
}