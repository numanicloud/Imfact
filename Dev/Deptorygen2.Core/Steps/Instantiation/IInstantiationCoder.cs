namespace Deptorygen2.Core.Steps.Instantiation
{
	internal interface IInstantiationCoder
	{
		InstantiationMethod Method { get; }
		string? GetCode(InstantiationRequest request, IInstantiationResolver resolver);
	}
}