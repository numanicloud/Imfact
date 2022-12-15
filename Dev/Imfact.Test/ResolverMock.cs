using Imfact.Steps.Filter.Wrappers;
using Microsoft.CodeAnalysis;

namespace Imfact.Test;

internal class ResolverMock : IResolverWrapper
{
	public bool IsIndirectResolverMutable { get; set; } = true;
    public Accessibility Accessibility { get; set; } = Accessibility.Public;

	public bool IsIndirectResolver() => IsIndirectResolverMutable;
}
