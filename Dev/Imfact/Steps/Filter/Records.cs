using Imfact.Utilities;
using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Filter;

internal record FilteredType(INamedTypeSymbol Symbol, RecordArray<FilteredMethod> Methods);

internal record FilteredMethod(IMethodSymbol Symbol);