using Imfact.Interfaces;
using Imfact.Steps.Aspects.Rules;
using Imfact.Steps.Ranking;

namespace Imfact.Steps.Aspects
{
	internal sealed class AspectStep
	{
		private readonly ClassRule _classRule;

		public AspectStep(IAnalysisContext context)
		{
			_classRule = new ClassRule(context);
		}

		public ClassAspect Run(RankedClass rankedClass)
		{
			return _classRule.Aggregate(rankedClass);
		}
	}
}
