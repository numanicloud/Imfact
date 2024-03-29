﻿using Imfact.Steps.Aspects.Rules;
using Imfact.Steps.Ranking;

namespace Imfact.Steps.Aspects
{
	internal sealed class AspectStep
	{
		private readonly ClassRule _rule;

		public AspectStep(ClassRule rule)
		{
			_rule = rule;
		}

		public AspectResult Run(RankedClass rankedClass)
		{
			return new AspectResult(_rule.Aggregate(rankedClass));
		}
	}
}
