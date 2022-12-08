using Imfact.Main;
using Imfact.Steps.Aspects;
using Imfact.Steps.Semanticses.Rules;

namespace Imfact.Steps.Semanticses;

internal sealed class SemanticsStep
{
	private readonly FactoryRule _factoryRule;
	private readonly GenerationContext _genContext;

	public SemanticsStep(FactoryRule rule, GenerationContext genContext)
	{
		_factoryRule = rule;
		_genContext = genContext;
	}

	public SemanticsResult Run(AspectResult aspectResult)
	{
		using var profiler = _genContext.Profiler.GetScope();

		var factory = _factoryRule.ExtractFactory(aspectResult.Class);
		return new SemanticsResult(factory);
	}
}