using Deptorygen2.Core.Steps.Aspects;

namespace Deptorygen2.Core.Steps.Semanticses.Rules
{
	internal sealed class SemanticsRule
	{
		private readonly FactoryRule _factoryRule;

		public SemanticsRule()
		{
			_factoryRule = new FactoryRule(new ResolverRule());
		}

		public SemanticsRoot Aggregate(ClassAspect @class)
		{
			var factory = _factoryRule.ExtractFactory(@class);
			return new SemanticsRoot(factory);
		}
	}
}
