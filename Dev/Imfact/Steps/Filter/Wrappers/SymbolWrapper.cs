using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Filter.Wrappers;

internal class FactorySymbolWrapper : IFactoryClassWrapper
{
    public required INamedTypeSymbol Symbol { get; init; }
}

internal class ResolverSymbolWrapper : IResolverWrapper
{
    public required IMethodSymbol Symbol { get; init; }
}

internal class ReturnTypeSymbolWrapper : IReturnTypeWrapper
{
    public required INamedTypeSymbol Symbol { get; init; }
}

internal class AttributeSymbolWrapper : IAttributeWrapper
{
    public required AttributeData Data { get; init; }
    public required INamedTypeSymbol Symbol { get; init; }
}

internal class BaseFactorySymbolWrapper : IBaseFactoryWrapper
{
    public required INamedTypeSymbol Symbol { get; init; }
}

internal class ResolutionSymbolWrapper : IResolutionFactoryWrapper
{
    public required INamedTypeSymbol Symbol { get; init; }
}