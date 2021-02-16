using System.Collections.Generic;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Interfaces
{
	internal interface IServiceConsumer : ISemanticsNode
	{
		IEnumerable<TypeName> GetRequiredServiceTypes();
	}
}
