using Imfact.Steps.Filter;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Cacheability;

internal sealed class CacheabilityStep
{
	private static CacheabilityStep? _instance;
	public static CacheabilityStep Instance => _instance ??= new CacheabilityStep();

	private CacheabilityStep()
	{
	}

	public CacheabilityResult Transform(FilteredType type, CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();

		var baseType = type.Symbol.BaseType is not { } b
			? null
			: TransformToDependency(b);

		var dependencies = type.Methods
			.Select(x => x.Symbol.ReturnType is not INamedTypeSymbol n
				? null
				: TransformToDependency(n))
			.FilterNull();

		return new CacheabilityResult(type,
			baseType,
			new RecordArray<Dependency>(dependencies.ToArray()));

		static Dependency? TransformToDependency(INamedTypeSymbol named)
		{
			foreach (var attributeData in named.GetAttributes())
			{
				if (IsFactoryAttribute(attributeData))
				{
					return new Dependency(named);
				}
			}
			return null;
		}

		static bool IsFactoryAttribute(AttributeData attribute)
		{
			return attribute.AttributeClass?.Name
				== "Imfact.Annotations.FactoryAttribute";
		}
	}
}