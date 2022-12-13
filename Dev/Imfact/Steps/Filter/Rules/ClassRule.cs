using Imfact.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Imfact.Steps.Filter.Rules;

internal sealed class ClassRule
{
    public required MethodRule MethodRule { private get; init; }

    public FilteredType? TransformClass(TypeDeclarationSyntax type,
        GeneratorSyntaxContext context,
        CancellationToken ct)
    {
        if (type.Modifiers.IndexOf(SyntaxKind.PartialKeyword) == -1) return null;

        var symbol = context.SemanticModel.GetDeclaredSymbol(type, ct);
        if (symbol is null) return null;

        var methods = FilteredMethods(type, context, ct);
        var baseTypes = FilterBaseTypes(symbol, ct);
        var resolutions = FilterResolutions(methods, ct);
        var delegations = FilterDelegations(symbol, ct);

        return new FilteredType(symbol,
            methods.ToRecordArray(),
            baseTypes.ToRecordArray(),
            resolutions.ToRecordArray(),
            delegations.ToRecordArray());
    }

    public FilteredType? Match(FilteredType type, AnnotationContext annotations, CancellationToken ct)
    {
        if (!GeneralRule.Instance.IsFactoryCandidate(type.Symbol, annotations))
            return null;

        return type with
        {
            BaseFactories = type.BaseFactories
                .Where(x => GeneralRule.Instance.IsFactoryReference(x.Symbol, annotations))
                .ToArray()
                .ToRecordArray(),

            ResolutionFactories = type.ResolutionFactories
                .Where(x => GeneralRule.Instance.IsFactoryReference(x.Symbol, annotations))
                .ToArray()
                .ToRecordArray(),

            Delegations = type.Delegations
                .Where(x => GeneralRule.Instance.IsFactoryReference(x.Symbol.Type, annotations))
                .ToArray()
                .ToRecordArray(),

            Methods = type.Methods
                .Select(m => MethodRule.Match(m, annotations, ct))
                .ToArray()
                .ToRecordArray()
        };
    }

    private FilteredMethod[] FilteredMethods(
        TypeDeclarationSyntax syntax,
        GeneratorSyntaxContext context,
        CancellationToken ct)
    {
        return syntax.Members
            .OfType<MethodDeclarationSyntax>()
            .Select(x => MethodRule.TransformResolver(x, context, ct))
            .FilterNull()
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
}