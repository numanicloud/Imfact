using System.Collections.Generic;

namespace Deptorygen2.Core.Interfaces
{
	interface IServiceProvider
	{
		IEnumerable<TypeName> GetCapableServiceTypes();
	}
}
