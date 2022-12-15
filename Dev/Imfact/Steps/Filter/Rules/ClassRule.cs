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

        var methods = FilteredMethods(type, context, ct);
        var baseTypes = FilterBaseTypes(symbol, ct);
        var resolutions = FilterResolutions(methods, ct);
        var delegations = FilterDelegations(symbol, ct);

        return new FilteredType(new FactorySymbolWrapper { Symbol = symbol },
            methods.ToRecordArray(),
            baseTypes.ToRecordArray(),
            resolutions.ToRecordArray(),
            delegations.ToRecordArray());
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
            BaseFactories = type.BaseFactories
                .Where(x => IsFactoryReference(x.Wrapper))
                .ToArray()
                .ToRecordArray(),

            ResolutionFactories = type.ResolutionFactories
                .Where(x => IsFactoryReference(x.Type))
                .ToArray()
                .ToRecordArray(),

            Delegations = type.Delegations
                .Where(x => IsFactoryReference(x.Wrapper))
                .ToArray()
                .ToRecordArray(),

            Methods = type.Methods
                .Select(m => MethodRule.Match(m, annotations, ct))
                .ToArray()
                .ToRecordArray()
        };

		bool IsFactoryReference(ITypeWrapper reference) =>
			reference.GetAttributes()
				.Any(attr => annotations.FactoryAttribute.IsInSameModuleWith(reference)
					? annotations.FactoryAttribute.IsUsedAs(attr)
					: attr.FullName == factoryAttributeFullName);
	}

    private bool IsFactoryCandidate(IFactoryClassWrapper symbol, AnnotationContext annotations)
    {
        return symbol.GetAttributes()
            .Any(annotations.FactoryAttribute.IsUsedAs);
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

    private FilteredBaseType[] FilterBaseTypes(INamedTypeSymbol symbol, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

		return TraverseBase(symbol)
            .Select(x => new BaseFactorySymbolWrapper { Symbol = x })
            .Where(x => x.IsConstructableClass)
            .Select(x => new FilteredBaseType(x, FilterIndirectResolvers(x)))
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

        FilteredMethod[] FilterIndirectResolvers(BaseFactorySymbolWrapper x)
        {
            ct.ThrowIfCancellationRequested();
            return x.Symbol.GetMembers()
                .OfType<IMethodSymbol>()
                .Select(MethodRule.TransformIndirectResolver)
                .FilterNull()
                .ToArray();
        }
    }

    private static FilteredResolution[] FilterResolutions(FilteredMethod[] methods, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return methods
            .Select(x => x.ReturnType)
            .Where(x => x.IsConstructableClass)
            .Select(x => new FilteredResolution(x))
            .ToArray();
    }

    private static FilteredDelegation[] FilterDelegations(
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
			.Where(x => x.Wrapper.IsConstructableClass)
            .ToArray();
	}
}