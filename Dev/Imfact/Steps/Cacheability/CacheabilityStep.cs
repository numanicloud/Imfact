using Imfact.Steps.Filter;

namespace Imfact.Steps.Cacheability;

internal sealed class CacheabilityStep
{
	private static CacheabilityStep? _instance;
	public static CacheabilityStep Instance => _instance ??= new CacheabilityStep();

	private CacheabilityStep()
	{
	}

	public FilteredType[] Transform(FilteredType type, CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();

		return new[] { type };
	}
}