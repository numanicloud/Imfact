using Imfact.Steps.Filter.Wrappers;
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

        // TODO: Testabilityのために、フィルタリングはTransformではやらずMatchでやる
        var methods = TransformMethods(type, context, ct);
        var baseTypes = TransformBaseType(symbol, ct);
        var resolutions = TransformResolutions(methods, ct);
        var delegations = TransformDelegations(symbol, ct);
        
        return new FilteredType(new FactorySymbolWrapper { Symbol = symbol },
            methods,
            baseTypes,
            resolutions,
            delegations);
    }

    public FilteredType? Match(FilteredType type, AnnotationContext annotations, CancellationToken ct)
	{
        if (!IsFactoryCandidate(type.Symbol, annotations))
            return null;

        var factoryAttributeFullName = annotations.FactoryAttribute.GetFullNameSpace()
            + "."
            + annotations.FactoryAttribute.Name;

        return type with
        {
            BaseFactory = type.BaseFactory is not { } b
				? null
				: MatchBaseType(b),

            ResolutionFactories = type.ResolutionFactories
                .Where(x => IsFactoryReference(x.Type))
                .ToArray(),

            Delegations = type.Delegations
                .Where(x => IsFactoryReference(x.Wrapper))
                .ToArray(),

            Methods = type.Methods
                .Select(m => MethodRule.Match(m, annotations, ct))
                .ToArray()
        };

        bool IsFactoryReference(ITypeWrapper reference) =>
            reference.IsConstructableClass
             && reference.GetAttributes()
	                .Any(attr => annotations.FactoryAttribute.IsInSameModuleWith(reference)
	                    ? annotations.FactoryAttribute.IsUsedAs(attr)
	                    : attr.FullName == factoryAttributeFullName);

        FilteredBaseType? MatchBaseType(FilteredBaseType pivot) =>
			pivot.BaseFactory is not { } baseFactory
				? null
				: !IsFactoryReference(baseFactory.Wrapper)
					? null
					: pivot with { BaseFactory = MatchBaseType(baseFactory) };
	}

    private bool IsFactoryCandidate(IFactoryClassWrapper symbol, AnnotationContext annotations)
    {
        return symbol.GetAttributes()
            .Any(annotations.FactoryAttribute.IsUsedAs);
    }

    private FilteredMethod[] TransformMethods(
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

    private FilteredBaseType? TransformBaseType(INamedTypeSymbol symbol, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        return Traverse(symbol);

        FilteredBaseType? Traverse(INamedTypeSymbol pivot)
        {
            if (pivot.BaseType is not { } baseType) return null;

            return new FilteredBaseType(new BaseFactorySymbolWrapper { Symbol = baseType },
                TransformIndirectResolvers(baseType),
                Traverse(baseType));
        }

        FilteredMethod[] TransformIndirectResolvers(INamedTypeSymbol x)
        {
            ct.ThrowIfCancellationRequested();
            return x.GetMembers()
                .OfType<IMethodSymbol>()
                .Select(MethodRule.TransformResolver)
                .FilterNull()
                .ToArray();
        }
    }

	private static FilteredResolution[] TransformResolutions(FilteredMethod[] methods, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return methods
            .Select(x => x.ReturnType)
            .Select(x => new FilteredResolution(x))
            .ToArray();
    }

    private static FilteredDelegation[] TransformDelegations(
        INamedTypeSymbol owner,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return owner.GetMembers()
            .OfType<IPropertySymbol>()
            .Select(property => property.Type is not INamedTypeSymbol named
                ? null
                : new FilteredDelegation(new DelegationSymbolWrapper { Symbol = named }))
            .FilterNull()
            .ToArray();
    }
}