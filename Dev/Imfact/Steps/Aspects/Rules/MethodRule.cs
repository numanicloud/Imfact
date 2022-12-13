using Imfact.Entities;
using Imfact.Steps.Filter;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Aspects.Rules;

internal sealed class MethodRule
{
    public required AttributeRule AttributeRule { private get; init; }
    public required TypeRule TypeRule { private get; init; }

    public MethodAspect? ExtractAspect(IMethodSymbol symbol)
    {
        using var profiler = AggregationProfiler.GetScope();
        try
        {
            if (symbol.ReturnType is not INamedTypeSymbol returnSymbol)
            {
                return null;
            }

            if (symbol.Parameters.Any(x => SymbolEqualityComparer.Default.Equals(x.Type, returnSymbol)))
            {
                throw new InvalidOperationException();
            }

            return new MethodAspect(symbol.Name,
                symbol.DeclaredAccessibility,
                GetKind(returnSymbol),
                GetReturnType(returnSymbol),
                GetAttributes(symbol, returnSymbol),
                GetParameters(symbol));
        }
        catch (Exception ex)
        {
            throw new Exception(
                $"Exception occured in extracting aspect of a method {symbol.Name}.",
                ex);
        }
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

    public MethodAspect ExtractAspect(FilteredMethod method)
    {
        var comparer = SymbolEqualityComparer.Default;
        if (method.Symbol.Parameters.Any(x => comparer.Equals(x.Type, method.ReturnType)))
        {
            // 開発段階で避けるべき無限再帰エラー
            throw new InvalidOperationException();
        }

        return new MethodAspect(method.Symbol.Name,
            method.Symbol.DeclaredAccessibility,
            GetKind(method.ReturnType),
            GetReturnType(method.ReturnType),
            GetAttributes(method),
            GetParameters(method.Symbol));
    }

	private MethodAttributeAspect[] GetAttributes(FilteredMethod method)
	{
		return method.Attributes
			.Select(x => AttributeRule.ExtractAspect(x, method.ReturnType, method.Symbol.Name))
			.ToArray();
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