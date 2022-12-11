using Imfact.Steps.Filter;
using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Cacheability;

internal record CacheabilityResult(FilteredType Type, string? SubTitle);

internal record Dependency(INamedTypeSymbol Symbol);