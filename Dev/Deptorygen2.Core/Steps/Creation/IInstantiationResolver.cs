using System.Collections.Generic;

namespace Deptorygen2.Core.Steps.Instantiation
{
	internal interface IInstantiationResolver
	{
		string? GetInjection(InstantiationRequest request);
		IEnumerable<string> GetInjections(MultipleInstantiationRequest request);
	}
}