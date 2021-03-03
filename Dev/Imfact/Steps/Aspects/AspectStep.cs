using Imfact.Steps.Aspects.Rules;
using Imfact.Steps.Ranking;

namespace Imfact.Steps.Aspects
{
	internal sealed class AspectStep
	{
		private readonly GenerationContext _genContext;

		public AspectStep(GenerationContext genContext)
		{
			_genContext = genContext;
		}

		public ClassAspect Run(RankedClass rankedClass)
		{
			var classRule = new ClassRule(rankedClass.Context, _genContext);
			return classRule.Aggregate(rankedClass);
		}
	}
}
