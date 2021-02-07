using System.Collections.Generic;
using Deptorygen.Utilities;

namespace Deptorygen.Generator.Interfaces
{
	interface IServiceConsumer
	{
		IEnumerable<TypeName> GetRequiredServiceTypes();
	}
}
