using Imfact.Annotations;
using Microsoft.CodeAnalysis;
using System.Threading;

namespace Imfact;

internal sealed class AnnotationContext
{
    public required INamedTypeSymbol FactoryAttribute { get; init; }
    public required INamedTypeSymbol ResolutionAttribute { get; init; }
    public required INamedTypeSymbol ResolutionAttributeT { get; init; }
    public required INamedTypeSymbol HookAttribute { get; init; }
    public required INamedTypeSymbol ExporterAttribute { get; init; }
    public required INamedTypeSymbol CacheAttribute { get; init; }
    public required INamedTypeSymbol CachePerResolutionAttribute { get; init; }
    public required INamedTypeSymbol TransientAttribute { get; init; }
    public required INamedTypeSymbol Cache { get; init; }
    public required INamedTypeSymbol CachePerResolution { get; init; }

    public static AnnotationContext FromCompilation(Compilation compilation, CancellationToken ct)
    {
        var ns = AnnotationDefinitions.NamespaceName;
        return new AnnotationContext
        {
            FactoryAttribute = EnsureGetType(AnnotationDefinitions.FactoryAttributeName),
            ResolutionAttribute = EnsureGetType(AnnotationDefinitions.ResolutionAttributeName),
            ResolutionAttributeT = EnsureGetType(AnnotationDefinitions.ResolutionAttributeName + "`1"),
            HookAttribute = EnsureGetType(AnnotationDefinitions.HookAttributeName),
            ExporterAttribute = EnsureGetType(AnnotationDefinitions.ExporterAttributeName),
            CacheAttribute = EnsureGetType(AnnotationDefinitions.CacheAttributeName),
            CachePerResolutionAttribute = EnsureGetType(AnnotationDefinitions.CachePerResolutionAttributeName),
            TransientAttribute = EnsureGetType(AnnotationDefinitions.TransientAttributeName),
            Cache = EnsureGetType(AnnotationDefinitions.CacheName),
            CachePerResolution = EnsureGetType(AnnotationDefinitions.CachePerResolutionAttributeName)
        };

        INamedTypeSymbol EnsureGetType(string typeName)
        {
            ct.ThrowIfCancellationRequested();
            return compilation.GetTypeByMetadataName($"{ns}.{typeName}")
                ?? throw new NullReferenceException($"{ns}.{typeName} not found.");
        }
    }
}