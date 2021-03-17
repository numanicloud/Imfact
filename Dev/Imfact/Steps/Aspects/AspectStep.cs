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
			var context = rankedClass.Context;
			var typeRule = new TypeRule();
			var methodRule = new MethodRule(context,
				new AttributeRule(typeRule),
				typeRule);
			var k = new ClassRule(_genContext,
				methodRule,
				new PropertyRule(context, methodRule));

			return k.Aggregate(rankedClass);
		}
	}
}
