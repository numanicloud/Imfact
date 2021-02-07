using System.Collections.Generic;
using Deptorygen.Utilities;

namespace Deptorygen.Generator.Interfaces
{
	interface IServiceProvider
	{
		IEnumerable<TypeName> GetCapableServiceTypes();
	}
}
