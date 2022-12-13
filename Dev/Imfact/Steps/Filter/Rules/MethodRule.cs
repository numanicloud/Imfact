using Imfact.Entities;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Imfact.Steps.Filter.Rules;

internal sealed class MethodRule
{
    public FilteredMethod? TransformResolver(MethodDeclarationSyntax method,
        GeneratorSyntaxContext context,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
		if (!GeneralRule.Instance.IsResolverToGenerate(method)) return null;

        return context.SemanticModel.GetDeclaredSymbol(method, ct) is { } ms
            ? ms.ReturnType is INamedTypeSymbol returnType
				? new FilteredMethod(ms, returnType, ExtractAttributes(ms).ToRecordArray())
                : null
            : null;
    }

    public FilteredMethod Match(
		FilteredMethod method,
        AnnotationContext annotations,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return method with
        {
            Attributes = method.Attributes
                .Select(x => TransformAttribute(x, annotations))
                .FilterNull()
                .ToArray()
                .ToRecordArray()
        };
    }

    private static FilteredAttribute[] ExtractAttributes(IMethodSymbol method)
    {
        // AnnotationKind.Hook を使っているが、本当は Unidentified のような名前を使うべき
        return method.GetAttributes()
            .Select(x => new FilteredAttribute(x, AnnotationKind.Hook))
            .ToArray();
    }

    private static FilteredAttribute? TransformAttribute(
        FilteredAttribute source,
        AnnotationContext annotations)
    {
        if (source.Data.AttributeClass is not { } attr) return null;

        var context = new AttributeExtractionContext
        {
            Annotations = annotations,
            Attribute = attr,
            AttributeData = source.Data,
            OriginalDefinition = attr.OriginalDefinition
        };

        return GetResolutionAttribute(in context)
            ?? GetResolutionTAttribute(in context)
            ?? GetHookAttribute(in context)
            ?? GetCacheAttribute(in context)
            ?? GetCachePerResolutionAttribute(in context);
    }

    private static FilteredAttribute? GetResolutionAttribute(in AttributeExtractionContext context)
    {
        if (!SymbolEquals(context.OriginalDefinition, context.Annotations.ResolutionAttribute))
            return null;

        var data = context.AttributeData;
        if (data.ConstructorArguments.Length == 1
            && data.ConstructorArguments[0].Kind == TypedConstantKind.Type
            && data.ConstructorArguments[0].Value is INamedTypeSymbol type)
        {
            return new ResolutionAttribute(data, type, AnnotationKind.Resolution);
        }

        return null;
    }

    private static FilteredAttribute? GetResolutionTAttribute(in AttributeExtractionContext context)
    {
        if (!SymbolEquals(context.OriginalDefinition, context.Annotations.ResolutionAttributeT))
            return null;

        var attr = context.Attribute;
        if (attr.TypeArguments.Length == 1
            && attr.TypeArguments[0] is INamedTypeSymbol type)
        {
            return new ResolutionAttribute(context.AttributeData, type, AnnotationKind.Resolution);
        }

        return null;
    }

    private static FilteredAttribute? GetHookAttribute(in AttributeExtractionContext context)
    {

        if (!SymbolEquals(context.OriginalDefinition, context.Annotations.HookAttribute))
            return null;

        var attr = context.Attribute;
        if (attr.TypeArguments.Length == 1
            && attr.TypeArguments[0] is INamedTypeSymbol type)
        {
            return new HookAttribute(context.AttributeData, type, AnnotationKind.Hook);
        }

        return null;
    }

    private static FilteredAttribute? GetCacheAttribute(in AttributeExtractionContext context)
    {
        var annotations = context.Annotations;

        return SymbolEquals(context.OriginalDefinition, annotations.CacheAttribute)
            ? new HookAttribute(context.AttributeData, annotations.Cache, AnnotationKind.CacheHookPreset)
            : null;
    }

    private static FilteredAttribute? GetCachePerResolutionAttribute(in AttributeExtractionContext context)
    {
        var annotations = context.Annotations;

        return SymbolEquals(context.OriginalDefinition, annotations.CachePerResolutionAttribute)
            ? new HookAttribute(context.AttributeData, annotations.CachePerResolution, AnnotationKind.CachePrHookPreset)
            : null;
    }

    private static bool SymbolEquals(ISymbol? x, ISymbol? y)
    {
        return SymbolEqualityComparer.Default.Equals(x, y);
    }

    private readonly struct AttributeExtractionContext
    {
        public required INamedTypeSymbol OriginalDefinition { get; init; }
        public required AttributeData AttributeData { get; init; }
        public required INamedTypeSymbol Attribute { get; init; }
        public required AnnotationContext Annotations { get; init; }
    }
}