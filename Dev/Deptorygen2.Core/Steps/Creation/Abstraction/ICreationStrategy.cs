namespace Deptorygen2.Core.Steps.Creation.Abstraction
{
	internal interface ICreationStrategy
	{
		string? GetCode(CreationRequest request, ICreationAggregator aggregator);
	}
}