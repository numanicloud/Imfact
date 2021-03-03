using Deptorygen2.Core.Steps.Aspects;

namespace Deptorygen2.Core.Steps.Semanticses.Rules
{
	internal sealed class SemanticsStep
	{
		private readonly FactoryRule _factoryRule;

		public SemanticsStep()
		{
			_factoryRule = new FactoryRule(new ResolverRule());
		}

		public SemanticsRoot Run(ClassAspect @class)
		{
			var factory = _factoryRule.ExtractFactory(@class);
			return new SemanticsRoot(factory);
		}
	}
}
