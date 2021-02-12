namespace Deptorygen2.Core.Steps.Creation.Strategies
{
	internal interface ICreationStrategy
	{
		string? GetCode(CreationRequest request, ICreationAggregator aggregator);
	}
}