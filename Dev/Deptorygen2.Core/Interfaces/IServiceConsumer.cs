using System.Collections.Generic;

namespace Deptorygen2.Core.Interfaces
{
	interface IServiceConsumer
	{
		IEnumerable<TypeName> GetRequiredServiceTypes();
	}
}
