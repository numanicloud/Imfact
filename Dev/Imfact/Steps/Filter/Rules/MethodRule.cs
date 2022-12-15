using Imfact.Entities;
using Imfact.Steps.Filter.Wrappers;
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

        if (context.SemanticModel.GetDeclaredSymbol(method, ct) is not { } ms) return null;

        if (ms.ReturnType is not INamedTypeSymbol returnType) return null;

        return new FilteredMethod(new ResolverSymbolWrapper { Symbol = ms },
            new ReturnTypeSymbolWrapper() { Symbol = returnType },
            ExtractAttributes(ms).ToRecordArray());
    }

    public FilteredMethod? TransformIndirectResolver(IMethodSymbol m)
    {
        if (m.ReturnType is not INamedTypeSymbol { SpecialType: SpecialType.None } returnType)
            return null;

        return new FilteredMethod(new ResolverSymbolWrapper { Symbol = m },
            new ReturnTypeSymbolWrapper { Symbol = returnType },
            RecordArray<FilteredAttribute>.Empty);
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
        // TODO: AnnotationKind.Hook を使っているが、本当は Unidentified のような名前を使うべき
        return method.GetAttributes()
            .Select(x => new AnnotationSymbolWrapper { Data = x })
            .Select(x => new FilteredAttribute(x, AnnotationKind.Hook))
            .ToArray();
    }

    private static FilteredAttribute? TransformAttribute(
        FilteredAttribute source,
        AnnotationContext annotations)
    {
        var context = new AttributeExtractionContext
        {
            Annotation = source.Wrapper,
            Master = annotations,
        };

        return GetResolutionAttribute(in context)
            ?? GetResolutionTAttribute(in context)
            ?? GetHookAttribute(in context)
            ?? GetCacheAttribute(in context)
            ?? GetCachePerResolutionAttribute(in context);
    }

    private static FilteredAttribute? GetResolutionAttribute(in AttributeExtractionContext context)
    {
        if (!SymbolEquals(context.Annotation, context.Master.ResolutionAttribute))
            return null;

        var data = context.Annotation;
        if (data.GetSingleConstructorArgumentAsType() is {} type)
        {
            return new ResolutionAttribute(data, type, AnnotationKind.Resolution);
        }

        return null;
    }

    private static FilteredAttribute? GetResolutionTAttribute(in AttributeExtractionContext context)
    {
        if (!SymbolEquals(context.Annotation, context.Master.ResolutionAttributeT))
            return null;
        
        if (context.Annotation.GetSingleTypeArgument() is { } type)
        {
            return new ResolutionAttribute(context.Annotation, type, AnnotationKind.Resolution);
        }

        return null;
    }

    private static FilteredAttribute? GetHookAttribute(in AttributeExtractionContext context)
    {

        if (!SymbolEquals(context.Annotation, context.Master.HookAttribute))
            return null;
        
        if (context.Annotation.GetSingleTypeArgument() is {} type)
        {
            return new HookAttribute(context.Annotation, type, AnnotationKind.Hook);
        }

        return null;
    }

    private static FilteredAttribute? GetCacheAttribute(in AttributeExtractionContext context)
    {
        var master = context.Master;

        return SymbolEquals(context.Annotation, master.CacheAttribute)
            ? new HookAttribute(context.Annotation, master.Cache, AnnotationKind.CacheHookPreset)
            : null;
    }

    private static FilteredAttribute? GetCachePerResolutionAttribute(in AttributeExtractionContext context)
    {
        var master = context.Master;

        return SymbolEquals(context.Annotation, master.CachePerResolutionAttribute)
            ? new HookAttribute(context.Annotation, master.CachePerResolution, AnnotationKind.CachePrHookPreset)
            : null;
    }

    private static bool SymbolEquals(IAnnotationWrapper? x, IAttributeWrapper y)
    {
        return x is not null && y.IsUsedAs(x);
    }

    private readonly struct AttributeExtractionContext
    {
        public required IAnnotationWrapper Annotation { get; init; }
        public required AnnotationContext Master { get; init; }
    }
}