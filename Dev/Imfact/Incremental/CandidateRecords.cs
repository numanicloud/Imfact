using Microsoft.CodeAnalysis;

namespace Imfact.Incremental;

internal record GenerationSource(FactoryCandidate[] Factories, AnnotationContext Annotations);

internal record FactoryCandidate
    (INamedTypeSymbol Symbol,
    ResolverCandidate[] Methods,
    AnnotationContext Annotations);

internal record ResolverCandidate(IMethodSymbol Symbol, bool IsToGenerate);

internal record FactoryIncremental(INamedTypeSymbol Symbol, ResolverIncremental[] Methods);

internal record ResolverIncremental(IMethodSymbol Symbol, bool IsToGenerate);