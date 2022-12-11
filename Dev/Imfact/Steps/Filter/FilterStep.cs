using Imfact.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Imfact.Steps.Filter;

// クラスの解析とメソッドの解析を ClassRule, MethodRule なるクラスに分割したい
internal sealed class FilterStep
{
    public bool IsFactory(SyntaxNode node, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return GeneralRule.Instance.IsFactoryClassDeclaration(node);
    }

    public FilteredType? Transform(GeneratorSyntaxContext context, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var syntax = (context.Node as TypeDeclarationSyntax)!;
        if (syntax.Modifiers.IndexOf(SyntaxKind.PartialKeyword) == -1) return null;

        var symbol = context.SemanticModel.GetDeclaredSymbol(syntax, ct);
        if (symbol is null) return null;

        var methods = FilteredMethods(syntax, context, ct);
        var baseTypes = FilterBaseTypes(symbol, ct);
        var resolutions = FilterResolutions(methods, ct);
        var delegations = FilterDelegations(symbol, ct);

        return new FilteredType(symbol,
            methods.ToRecordArray(),
            baseTypes.ToRecordArray(),
            resolutions.ToRecordArray(),
            delegations.ToRecordArray());
    }

    private static FilteredMethod[] FilteredMethods(
        TypeDeclarationSyntax syntax,
        GeneratorSyntaxContext context,
        CancellationToken ct)
    {
        return syntax.Members
            .OfType<MethodDeclarationSyntax>()
            .Select(TransformResolver)
            .FilterNull()
            .ToArray();

        FilteredMethod? TransformResolver(MethodDeclarationSyntax method)
        {
            ct.ThrowIfCancellationRequested();
            return context.SemanticModel.GetDeclaredSymbol(method, ct) is { } ms
                ? new FilteredMethod(ms, ExtractAttributes(ms).ToRecordArray())
                : null;
        }
    }

    private static FilteredAttribute[] ExtractAttributes(IMethodSymbol method)
    {
        return method.GetAttributes()
            .Select(x => new FilteredAttribute(x))
            .ToArray();
    }

    private static FilteredDependency[] FilterBaseTypes(INamedTypeSymbol symbol, CancellationToken ct)
    {
        return TraverseBase(symbol)
            .Select(x => TransformDependency(x, ct))
            .FilterNull()
            .ToArray();

        static IEnumerable<INamedTypeSymbol> TraverseBase(INamedTypeSymbol pivot)
        {
            if (pivot.BaseType is not { } baseType) yield break;

            yield return baseType;
            foreach (var child in TraverseBase(baseType))
            {
                yield return child;
            }
        }
    }

    private static FilteredDependency[] FilterResolutions(FilteredMethod[] methods, CancellationToken ct)
    {
        return methods
            .Select(x => x.Symbol.ReturnType as INamedTypeSymbol)
            .FilterNull()
            .Select(x => TransformDependency(x, ct))
            .FilterNull()
            .ToArray();
    }

    private static FilteredDelegation[] FilterDelegations(
        INamedTypeSymbol owner,
        CancellationToken ct)
    {
        return owner.GetMembers()
            .OfType<IPropertySymbol>()
            .Select(TransformProperty)
            .FilterNull()
            .ToArray();

        FilteredDelegation? TransformProperty(IPropertySymbol property)
        {
            return property.Type is INamedTypeSymbol named
                ? TransformDependency(named, ct) is not null
                    ? new FilteredDelegation(property)
                    : null
                : null;
        }
    }

    private static FilteredDependency? TransformDependency(
        INamedTypeSymbol typeToConstruct,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return typeToConstruct.IsReferenceType
            && !typeToConstruct.IsRecord
            && !typeToConstruct.IsAbstract
                ? new FilteredDependency(typeToConstruct)
                : null;
    }

    public FilteredType? Match(
        (FilteredType? type, AnnotationContext annotations) tuple,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        if (tuple.type is null) return null;
        if (!GeneralRule.Instance.IsFactoryCandidate(tuple.type.Symbol, tuple.annotations))
            return null;

        var baseTypes = tuple.type.BaseFactories
            .Where(x => GeneralRule.Instance.IsFactoryReference(x.Symbol, tuple.annotations))
            .ToArray();

        var resolutions = tuple.type.ResolutionFactories
            .Where(x => GeneralRule.Instance.IsFactoryReference(x.Symbol, tuple.annotations))
            .ToArray();

        var delegations = tuple.type.Delegations
            .Where(x => GeneralRule.Instance.IsFactoryReference(x.Symbol.Type, tuple.annotations))
            .ToArray();

        return tuple.type with
        {
            BaseFactories = baseTypes.ToRecordArray(),
            ResolutionFactories = resolutions.ToRecordArray(),
            Delegations = delegations.ToRecordArray(),
            Methods = tuple.type.Methods
                .Select(m => m with
                {
                    Attributes = m.Attributes
                        .Select(x => TransformAttribute(x, tuple.annotations))
						.FilterNull()
						.ToArray()
                        .ToRecordArray()
                }).ToArray().ToRecordArray()
        };

        static FilteredAttribute? TransformAttribute(
            FilteredAttribute source,
            AnnotationContext annotations)
        {
            if (source.Data.AttributeClass is not { } attr) return null;

            var data = source.Data;
            var origin = attr.OriginalDefinition;
            var comparer = SymbolEqualityComparer.Default;

            // ?? を使ったResponsibilityChainパターンに置き換えられそう

            if (comparer.Equals(origin, annotations.ResolutionAttribute))
            {
                if (data.ConstructorArguments.Length == 1
                    && data.ConstructorArguments[0].Kind == TypedConstantKind.Type
                    && data.ConstructorArguments[0].Value is INamedTypeSymbol type)
                {
                    return new ResolutionAttribute(data, type);
                }
                return null;
            }

            if (comparer.Equals(origin, annotations.ResolutionAttributeT))
            {
                if (attr.TypeArguments.Length == 1
                    && attr.TypeArguments[0] is INamedTypeSymbol type)
                {
                    return new ResolutionAttribute(data, type);
                }
                return null;
            }

            if (comparer.Equals(origin, annotations.HookAttribute))
            {
                if (attr.TypeArguments.Length == 1
                    && attr.TypeArguments[0] is INamedTypeSymbol type)
                {
                    return new HookAttribute(data, type);
                }
                return null;
            }

            if (comparer.Equals(origin, annotations.CacheAttribute))
            {
                return new HookAttribute(data, annotations.Cache);
            }

            if (comparer.Equals(origin, annotations.CachePerResolutionAttribute))
            {
                return new HookAttribute(data, annotations.CachePerResolution);
            }

            return null;
        }
    }
}