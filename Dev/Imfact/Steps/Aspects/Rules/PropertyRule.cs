using Imfact.Entities;
using Microsoft.CodeAnalysis;
using System.Linq;
using Imfact.Annotations;
using Imfact.Utilities;

namespace Imfact.Steps.Aspects.Rules
{
    internal class PropertyRule
	{
		private readonly MethodRule _methodRule;

		public PropertyRule(MethodRule methodRule)
		{
			_methodRule = methodRule;
		}

		public PropertyAspect? ExtractAspect(IPropertySymbol symbol)
		{
			var isDelegation = symbol.Type.GetAttributes()
				.Select(x => x.AttributeClass)
				.FilterNull()
				.Select(x => new AttributeName(x.Name))
				.Any(x => x.NameWithAttributeSuffix == nameof(FactoryAttribute));
			if (!isDelegation)
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
}