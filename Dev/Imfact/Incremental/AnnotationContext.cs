using Microsoft.CodeAnalysis;

namespace Imfact;

internal sealed class AnnotationContext
{
    public required INamedTypeSymbol FactoryAttribute { get; init; }
    public required INamedTypeSymbol ResolutionAttribute { get; init; }
    public required INamedTypeSymbol HookAttribute { get; init; }
    public required INamedTypeSymbol ExporterAttribute { get; init; }
    public required INamedTypeSymbol CacheAttribute { get; init; }
    public required INamedTypeSymbol CachePerResolutionAttribute { get; init; }
    public required INamedTypeSymbol TransientAttribute { get; init; }
}