namespace Deptorygen2.Core.Steps.Instantiation
{
	internal interface IInstantiationCoder
	{
		string? GetCode(InstantiationRequest request, IInstantiationResolver resolver);
	}
}