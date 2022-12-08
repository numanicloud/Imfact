using Imfact.Entities;
using Microsoft.CodeAnalysis;
using Imfact.Utilities;
using Imfact.Main;

namespace Imfact.Steps.Aspects.Rules;

internal class PropertyRule
{
	private readonly MethodRule _methodRule;
    private readonly Logger _logger;

    public PropertyRule(MethodRule methodRule, Logger logger)
	{
		_methodRule = methodRule;
        this._logger = logger;
    }

	public PropertyAspect? ExtractAspect(IPropertySymbol symbol, AnnotationContext annotations)
	{
		if (!GeneralRule.Instance.IsDelegation(symbol, annotations, _logger))
        {
            return null;
        }

        var methods = symbol.Type.GetMembers().OfType<IMethodSymbol>()
			.Select(m => _methodRule.ExtractNotAsRootResolver(m))
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