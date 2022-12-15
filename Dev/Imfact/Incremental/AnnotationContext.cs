using Imfact.Annotations;
using Imfact.Steps.Filter.Wrappers;
using Microsoft.CodeAnalysis;

namespace Imfact;

internal sealed class AnnotationContext
{
    public required IAttributeWrapper FactoryAttribute { get; init; }
    public required IAttributeWrapper ResolutionAttribute { get; init; }
    public required IAttributeWrapper ResolutionAttributeT { get; init; }
    public required IAttributeWrapper HookAttribute { get; init; }
    public required IAttributeWrapper ExporterAttribute { get; init; }
    public required IAttributeWrapper CacheAttribute { get; init; }
    public required IAttributeWrapper CachePerResolutionAttribute { get; init; }
    public required IAttributeWrapper TransientAttribute { get; init; }
    public required ITypeWrapper Cache { get; init; }
    public required ITypeWrapper CachePerResolution { get; init; }

    public static AnnotationContext FromCompilation(Compilation compilation, CancellationToken ct)
    {
        var ns = AnnotationDefinitions.NamespaceName;
        return new AnnotationContext
        {
            FactoryAttribute = EnsureGetAttr(AnnotationDefinitions.FactoryAttributeName),
            ResolutionAttribute = EnsureGetAttr(AnnotationDefinitions.ResolutionAttributeName),
            ResolutionAttributeT = EnsureGetAttr(AnnotationDefinitions.ResolutionAttributeName + "`1"),
            HookAttribute = EnsureGetAttr(AnnotationDefinitions.HookAttributeName),
            ExporterAttribute = EnsureGetAttr(AnnotationDefinitions.ExporterAttributeName),
            CacheAttribute = EnsureGetAttr(AnnotationDefinitions.CacheAttributeName),
            CachePerResolutionAttribute = EnsureGetAttr(AnnotationDefinitions.CachePerResolutionAttributeName),
            TransientAttribute = EnsureGetAttr(AnnotationDefinitions.TransientAttributeName),
            Cache = EnsureGetType(AnnotationDefinitions.CacheName),
            CachePerResolution = EnsureGetType(AnnotationDefinitions.CachePerResolutionAttributeName)
        };

		IAttributeWrapper EnsureGetAttr(string typeName)
        {
            ct.ThrowIfCancellationRequested();
			var metadataName = $"{ns}.{typeName}";

			if (compilation.GetTypeByMetadataName(metadataName) is not { } symbol)
				throw new NullReferenceException($"{metadataName} not found.");

            return new AttributeSymbolWrapper
			{
                Symbol = symbol
			};
        }

		ITypeWrapper EnsureGetType(string typeName)
		{
			ct.ThrowIfCancellationRequested();
			var metadataName = $"{ns}.{typeName}";

			if (compilation.GetTypeByMetadataName(metadataName) is not { } symbol)
				throw new NullReferenceException($"{metadataName} not found.");

			return new TypeSymbolWrapper { Symbol = symbol };
		}
    }
}