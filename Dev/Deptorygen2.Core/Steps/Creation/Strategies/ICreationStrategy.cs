namespace Deptorygen2.Core.Steps.Instantiation
{
	internal interface ICreationStrategy
	{
		string? GetCode(CreationRequest request, ICreationAggregator aggregator);
	}
}