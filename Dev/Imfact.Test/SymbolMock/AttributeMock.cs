using Imfact.Steps.Filter.Wrappers;

namespace Imfact.Test.SymbolMock;

internal class AttributeMock : IAttributeWrapper
{
    private readonly string _fullNameSpace;
    public string Name { get; }

    public AttributeMock(string fullNameSpace, string name)
    {
        Name = name;
        _fullNameSpace = fullNameSpace;
    }

    public bool IsInSameModuleWith(ITypeWrapper other) => true;

    public bool IsUsedAs(IAnnotationWrapper annotation) =>
        annotation is AnnotationMock mock && mock.Attribute == this;

    public string GetFullNameSpace() => _fullNameSpace;
}
