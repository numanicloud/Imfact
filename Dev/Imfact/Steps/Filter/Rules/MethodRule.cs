using Imfact.Entities;
using Imfact.Steps.Filter.Wrappers;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Imfact.Steps.Filter.Rules;

internal sealed class MethodRule
{
    public IResolverWrapper? TransformResolver(MethodDeclarationSyntax method,
        GeneratorSyntaxContext context,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        return method.Modifiers.IndexOf(SyntaxKind.PartialKeyword) == -1
            ? null
            : context.SemanticModel.GetDeclaredSymbol(method, ct) is not { } ms
                ? null
                : TransformResolver(ms);
    }

    public IResolverWrapper? TransformResolver(IMethodSymbol method)
    {
        return method.ReturnType is not INamedTypeSymbol returnType
            ? null
            : new ResolverSymbolWrapper
            {
                Name = method.Name,
                Symbol = method,
                Accessibility = method.DeclaredAccessibility,
                ReturnType = new ReturnTypeSymbolWrapper()
                {
                    Symbol = returnType
                }
            };
    }

    public FilteredMethod? Match(
        IResolverWrapper method,
        AnnotationContext annotations,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var attrs = method.Annotations
            .Select(x => TransformAttribute(x, annotations))
            .FilterNull()
            .ToArray();
        return !method.IsIndirectResolver()
            ? null
            : new FilteredMethod
            {
                Name = method.Name,
                ReturnType = method.ReturnType.GetTypeAnalysis(),
                Attributes = method.Annotations
                    .Select(a => TransformAttribute(a, annotations))
                    .FilterNull()
                    .ToArray()
            };
    }

    public FilteredMethod? MatchAsInheritance(IResolverWrapper method,
        AnnotationContext annotations,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        return method.Accessibility is Accessibility.Public
            or Accessibility.Protected
            or Accessibility.Internal
            or Accessibility.ProtectedOrInternal
            or Accessibility.ProtectedAndInternal
            ? Match(method, annotations, ct)
            : null;
    }

    private static FilteredAttribute? TransformAttribute(
        IAnnotationWrapper source,
        AnnotationContext annotations)
    {
        var context = new AttributeExtractionContext
        {
            Annotation = source,
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
        if (data.GetSingleConstructorArgumentAsType() is { } type)
        {
            return new ResolutionAttribute(data.GetTypeAnalysis(), type, AnnotationKind.Resolution);
        }

        return null;
    }

    private static FilteredAttribute? GetResolutionTAttribute(in AttributeExtractionContext context)
    {
        var annotation = context.Annotation;

        if (!SymbolEquals(annotation, context.Master.ResolutionAttributeT))
            return null;

        if (annotation.GetSingleTypeArgument() is { } type)
        {
            return new ResolutionAttribute(annotation.GetTypeAnalysis(), type, AnnotationKind.Resolution);
        }

        return null;
    }

    private static FilteredAttribute? GetHookAttribute(in AttributeExtractionContext context)
    {

        if (!SymbolEquals(context.Annotation, context.Master.HookAttribute))
            return null;

        if (context.Annotation.GetSingleTypeArgument() is { } type)
        {
            return new HookAttribute(context.Annotation.GetTypeAnalysis(), type, AnnotationKind.Hook);
        }

        return null;
    }

    private static FilteredAttribute? GetCacheAttribute(in AttributeExtractionContext context)
    {
        var master = context.Master;

        return SymbolEquals(context.Annotation, master.CacheAttribute)
            ? new HookAttribute(context.Annotation.GetTypeAnalysis(), master.Cache, AnnotationKind.CacheHookPreset)
            : null;
    }

    private static FilteredAttribute? GetCachePerResolutionAttribute(in AttributeExtractionContext context)
    {
        var master = context.Master;

        return SymbolEquals(context.Annotation, master.CachePerResolutionAttribute)
            ? new HookAttribute(context.Annotation.GetTypeAnalysis(), master.CachePerResolution, AnnotationKind.CachePrHookPreset)
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