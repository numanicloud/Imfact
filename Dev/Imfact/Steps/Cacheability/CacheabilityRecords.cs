using Imfact.Steps.Filter;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Cacheability;

internal record CacheabilityResult(FilteredType Type,
	Dependency? BaseType,
	RecordArray<Dependency> Dependencies);

internal record Dependency(INamedTypeSymbol Symbol);