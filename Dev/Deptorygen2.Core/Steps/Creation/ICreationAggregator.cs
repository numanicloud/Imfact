using System.Collections.Generic;

namespace Deptorygen2.Core.Steps.Instantiation
{
	internal interface ICreationAggregator
	{
		string? GetInjection(CreationRequest request);
		IEnumerable<string> GetInjections(MultipleCreationRequest request);
	}
}