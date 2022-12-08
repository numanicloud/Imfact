using Imfact.Steps.Aspects;
using Imfact.Steps.Semanticses.Rules;
using Imfact.Utilities;

namespace Imfact.Steps.Semanticses;

internal sealed class SemanticsStep
{
	private readonly FactoryRule _factoryRule;

	public SemanticsStep(FactoryRule rule)
	{
		_factoryRule = rule;
	}

	public SemanticsResult Run(AspectResult aspectResult)
	{
		using var profiler = AggregationProfiler.GetScope();

		var factory = _factoryRule.ExtractFactory(aspectResult.Class);
		return new SemanticsResult(factory);
	}
}