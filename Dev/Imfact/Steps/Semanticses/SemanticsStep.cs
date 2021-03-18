using Imfact.Steps.Aspects;
using Imfact.Steps.Semanticses.Rules;

namespace Imfact.Steps.Semanticses
{
	internal sealed class SemanticsStep
	{
		private readonly FactoryRule _factoryRule;

		public SemanticsStep(FactoryRule rule)
		{
			_factoryRule = rule;
		}

		public SemanticsResult Run(AspectResult aspectResult)
		{
			var factory = _factoryRule.ExtractFactory(aspectResult.Class);
			return new SemanticsResult(factory);
		}
	}
}
