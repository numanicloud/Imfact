using Deptorygen2.Core.Steps.Aspects;
using Deptorygen2.Core.Steps.Semanticses.Nodes;

namespace Deptorygen2.Core.Steps.Semanticses.Rules
{
	internal sealed class SemanticsRule
	{
		private readonly FactoryRule _factoryRule;
		private readonly UsingRule _usingRule = new();
		private readonly DependencyRule _dependencyRule = new();

		public SemanticsRule()
		{
			_factoryRule = new FactoryRule(new ResolverRule());
		}

		public SemanticsRoot Aggregate(ClassAspect @class)
		{
			var factory = _factoryRule.ExtractFactory(@class);

			var dependency = _dependencyRule.Extract(factory);
			var disposableInfo = DisposableInfo.Aggregate(factory, dependency);
			var namespaces = _usingRule.Extract(factory, dependency, disposableInfo);
			return new SemanticsRoot(namespaces, factory, dependency, disposableInfo);
		}
	}
}
