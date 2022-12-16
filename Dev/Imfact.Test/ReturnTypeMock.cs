using Imfact.Entities;
using Imfact.Steps.Filter.Wrappers;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;

namespace Imfact.Test;

internal class ReturnTypeMock : IReturnTypeWrapper
{
    public bool IsConstructableClass { get; set; } = true;
	public IEnumerable<IAnnotationWrapper> AttributesMutable { get; set; }
	public TypeAnalysis TypeAnalysisMutable { get; set; }

	public ReturnTypeMock(string fullNamespace, string name)
	{
		AttributesMutable = Array.Empty<IAnnotationWrapper>();
		TypeAnalysisMutable = new TypeAnalysis(
			new TypeId(fullNamespace, name, RecordArray<TypeId>.Empty),
			Accessibility.Public,
			DisposableType.NonDisposable);
	}

	public IEnumerable<IAnnotationWrapper> GetAttributes() => AttributesMutable;

	public TypeAnalysis GetTypeAnalysis() => TypeAnalysisMutable;

    public IResolutionFactoryWrapper ToResolution()
    {
        return new ResolutionMock(TypeAnalysisMutable.FullNamespace, TypeAnalysisMutable.Name)
        {
			AttributeMutable = AttributesMutable
        };
    }
}
