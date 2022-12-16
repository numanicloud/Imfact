using Imfact.Entities;
using Imfact.Steps.Filter;
using Imfact.Steps.Filter.Wrappers;
using Imfact.Steps.Semanticses.Records;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Aspects.Rules;

internal sealed class MethodRule
{
    public required AttributeRule AttributeRule { private get; init; }
    public required TypeRule TypeRule { private get; init; }

    public MethodAspect? ExtractAspect(FilteredMethod symbol)
    {
        throw new NotImplementedException();
    }

    public MethodAspect? ExtractNotAsRootResolver(IMethodSymbol symbol)
    {
        if (symbol.ReturnType is not INamedTypeSymbol returnSymbol)
        {
            return null;
        }

        return new MethodAspect(symbol.Name,
            symbol.DeclaredAccessibility,
            GetKind(returnSymbol),
            GetReturnType(returnSymbol),
            GetAttributes(symbol, returnSymbol),
            GetParameters(symbol));
    }

    private ParameterAspect[] GetParameters(IMethodSymbol symbol)
    {
        return symbol.Parameters
            .Select(p => new ParameterAspect(TypeAnalysis.FromSymbol(p.Type), p.Name))
            .ToArray();
    }

    private MethodAttributeAspect[] GetAttributes(IMethodSymbol symbol, INamedTypeSymbol returnSymbol)
    {
        return symbol.GetAttributes()
            .Select(x => AttributeRule.ExtractAspect(x, returnSymbol, symbol.Name))
            .FilterNull()
            .ToArray();
    }

    private ReturnTypeAspect GetReturnType(INamedTypeSymbol symbol)
    {
        return new(TypeRule.ExtractTypeToCreate(symbol), symbol.IsAbstract);
    }

    private static ResolverKind GetKind(INamedTypeSymbol returnSymbol)
    {
        var idSymbol = returnSymbol.ConstructedFrom;
        var typeNameValid = idSymbol.MetadataName == typeof(IEnumerable<>).Name;
        var typeArgValid = idSymbol.TypeArguments.Length == 1;
        return typeNameValid && typeArgValid ? ResolverKind.Multi : ResolverKind.Single;
    }
}