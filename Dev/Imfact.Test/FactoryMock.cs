using Imfact.Entities;
using Imfact.Steps.Filter.Wrappers;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;

namespace Imfact.Test;

internal class FactoryMock : IFactoryClassWrapper
{
	private readonly TypeAnalysis _analysis;

	public IEnumerable<IAnnotationWrapper> AttributesMutable { get; set; }

	public IEnumerable<ITypeWrapper> AllInterfaces => Array.Empty<ITypeWrapper>();

	public FactoryMock(string fullNamespace, string name)
	{
		_analysis = new TypeAnalysis(new TypeId(fullNamespace, name, RecordArray<TypeId>.Empty),
			Accessibility.Public,
			DisposableType.NonDisposable);

		AttributesMutable = Array.Empty<IAnnotationWrapper>();
	}

	public IEnumerable<IAnnotationWrapper> GetAttributes() => AttributesMutable;

	public TypeAnalysis GetTypeAnalysis() => _analysis;

    public IResolverWrapper[] Methods { get; init; } = Array.Empty<IResolverWrapper>();

    public IBaseFactoryWrapper? BaseType { get; init; }

    public IResolutionFactoryWrapper[] Resolutions { get; init; } =
        Array.Empty<IResolutionFactoryWrapper>();

    public IDelegationFactoryWrapper[] Delegations { get; init; } = Array.Empty<IDelegationFactoryWrapper>();
}
