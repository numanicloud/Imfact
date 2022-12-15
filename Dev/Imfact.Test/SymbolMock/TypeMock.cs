using Imfact.Entities;
using Imfact.Steps.Filter.Wrappers;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;

namespace Imfact.Test.SymbolMock;

internal class TypeMock : ITypeWrapper
{
    private readonly TypeAnalysis _analysis;

    public bool IsConstructableClass => true;

    public TypeMock(string fullNamespace,
        string name,
        TypeId[] typeParameters)
    {
        _analysis = new TypeAnalysis(
            new TypeId(fullNamespace, name, typeParameters.ToRecordArray()),
            Accessibility.Public,
            DisposableType.NonDisposable);
    }

    public IEnumerable<IAnnotationWrapper> GetAttributes() =>
        Array.Empty<IAnnotationWrapper>();

    public TypeAnalysis GetTypeAnalysis() => _analysis;
}
