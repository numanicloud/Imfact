using Imfact.Entities;
using Imfact.Steps.Filter;
using Imfact.Steps.Filter.Wrappers;
using Microsoft.CodeAnalysis;
using Imfact.Utilities;

namespace Imfact.Steps.Aspects.Rules;

internal class PropertyRule
{
    public required MethodRule Rule { private get; init; }

    public PropertyAspect? ExtractAspect(IPropertySymbol symbol, AnnotationContext annotations)
    {
        using var profiler = AggregationProfiler.GetScope();

        var methods = symbol.Type.GetMembers().OfType<IMethodSymbol>()
            .Select(m => Rule.ExtractNotAsRootResolver(m))
            .FilterNull()
            .ToArray();

        var fields = symbol.ContainingType.GetMembers().OfType<IFieldSymbol>();
        var isAutoImplemented = fields
            .Any(f => SymbolEqualityComparer.Default.Equals(f.AssociatedSymbol, symbol));

        return new PropertyAspect(TypeAnalysis.FromSymbol(symbol.Type),
            symbol.Name,
            methods,
            isAutoImplemented);
    }

    public PropertyAspect[] ExtractAspect(FilteredType self, CancellationToken ct)
    {
        return self.Delegations
            .Select(x => ExtractAspect(x.Wrapper, ct))
            .ToArray();
    }

    private PropertyAspect ExtractAspect(IDelegationFactoryWrapper wrapper, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (wrapper is not DelegationSymbolWrapper { Symbol: var symbol })
        {
            throw new NotImplementedException();
        }

        // symbolがINamedTypeSymbolと化しているが、本当はIPropertyTypeだったのでこれは動作しない。
        // 今はリファクタリング中で仕方ないが、いずれ要らなくなるコードなので消す予定
		var methods = symbol.GetMembers()
            .OfType<IMethodSymbol>()
            .Select(Rule.ExtractNotAsRootResolver)
            .FilterNull()
            .ToArray();

        var isAutoImplemented = symbol.ContainingType.GetMembers()
            .OfType<IFieldSymbol>()
            .Any(f => SymbolEqualityComparer.Default.Equals(f.AssociatedSymbol, symbol));

        return new PropertyAspect(TypeAnalysis.FromSymbol(symbol),
            symbol.Name,
            methods,
            isAutoImplemented);
    }
}