using Imfact.Steps.Filter.Wrappers;
using Microsoft.CodeAnalysis;

namespace Imfact.Test;

internal class ResolverMock : IResolverWrapper
{
    public required IReturnTypeWrapper ReturnType { get; init; }
    public IEnumerable<IAnnotationWrapper> Annotations { get; init; } = Array.Empty<IAnnotationWrapper>();

	public bool IsIndirectResolverMutable { get; set; } = true;
    public Accessibility Accessibility { get; set; } = Accessibility.Public;
    public string Name { get; }

    public ResolverMock(string name)
    {
        Name = name;
    }

    public bool IsIndirectResolver() => IsIndirectResolverMutable;
}
