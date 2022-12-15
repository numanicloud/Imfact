using Imfact.Steps.Filter.Wrappers;

namespace Imfact.Test.SymbolMock;

internal class AnnotationMock : IAnnotationWrapper
{
    public AttributeMock Attribute { get; }
    private readonly ITypeWrapper? _ctorArg;
    private readonly ITypeWrapper? _typeArg;
    public string FullName => $"{Attribute.GetFullNameSpace()}.{Attribute.Name}";

    public AnnotationMock(AttributeMock attribute, ITypeWrapper? ctorArg, ITypeWrapper? typeArg)
    {
        Attribute = attribute;
        _ctorArg = ctorArg;
        _typeArg = typeArg;
    }

    public ITypeWrapper? GetSingleConstructorArgumentAsType() => _ctorArg;

    public ITypeWrapper? GetSingleTypeArgument() => _typeArg;
}
