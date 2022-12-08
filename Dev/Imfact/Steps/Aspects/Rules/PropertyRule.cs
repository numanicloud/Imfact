using Imfact.Entities;
using Microsoft.CodeAnalysis;
using Imfact.Utilities;
using Imfact.Main;

namespace Imfact.Steps.Aspects.Rules;

internal class PropertyRule
{
	public required MethodRule Rule { private get; init; }
	public required GenerationContext GenContext { private get; init; }

	public PropertyAspect? ExtractAspect(IPropertySymbol symbol, AnnotationContext annotations)
	{
		using var profiler = GenContext.Profiler.GetScope();

		if (!GeneralRule.Instance.IsDelegation(symbol, annotations, GenContext))
        {
            return null;
        }

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
}