using System.Collections.Generic;

namespace Deptorygen2.Core.Interfaces
{
	public interface INamespaceClaimer
	{
		IEnumerable<string> GetRequiredNamespaces();
	}
}