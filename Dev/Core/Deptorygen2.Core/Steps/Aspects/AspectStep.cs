using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Ranking;

namespace Deptorygen2.Core.Steps.Aspects
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
