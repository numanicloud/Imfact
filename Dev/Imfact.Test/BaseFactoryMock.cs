using Imfact.Entities;
using Imfact.Steps.Filter.Wrappers;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;

namespace Imfact.Test;

internal class BaseFactoryMock : IBaseFactoryWrapper
{
	public IEnumerable<IAnnotationWrapper> AttributesMutable { get; set; }
	public TypeAnalysis TypeAnalysisMutable { get; set; }
	public bool IsConstructableClass { get; set; } = true;
	public IEnumerable<ITypeWrapper> AllInterfaces { get; set; }

	public BaseFactoryMock(string fullNamespace, string name)
	{
		AttributesMutable = Array.Empty<IAnnotationWrapper>();
		TypeAnalysisMutable = new TypeAnalysis(
			new TypeId(fullNamespace, name, RecordArray<TypeId>.Empty),
			Accessibility.Public,
			DisposableType.NonDisposable);
		AllInterfaces = Array.Empty<ITypeWrapper>();
	}

	public IEnumerable<IAnnotationWrapper> GetAttributes() => AttributesMutable;

	public TypeAnalysis GetTypeAnalysis() => TypeAnalysisMutable;

    public IEnumerable<IResolverWrapper> Methods { get; init; } = Array.Empty<IResolverWrapper>();
    public IBaseFactoryWrapper? BaseType { get; init; }
}
