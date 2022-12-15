using Imfact.Steps.Filter.Wrappers;

namespace Imfact.Test;

internal class ResolverMock : IResolverWrapper
{
	public bool IsIndirectResolverMutable { get; set; } = true;

	public bool IsIndirectResolver() => IsIndirectResolverMutable;
}
