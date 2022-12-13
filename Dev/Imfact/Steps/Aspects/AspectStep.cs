using Imfact.Steps.Aspects.Rules;
using Imfact.Steps.Cacheability;
using Imfact.Steps.Ranking;

namespace Imfact.Steps.Aspects;

internal sealed class AspectStep
{
	public required ClassRule ClassRule { private get; init; }

	public AspectResult Run(RankedClass rankedClass)
	{
		return new AspectResult(ClassRule.Aggregate(rankedClass));
	}

	public AspectResult Transform(CacheabilityResult input, CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();
		return new AspectResult(ClassRule.Transform(input, ct));
	}
}