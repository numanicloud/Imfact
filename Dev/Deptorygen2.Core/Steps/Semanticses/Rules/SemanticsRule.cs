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

		public Generation Aggregate(ClassAspect @class)
		{
			var factory = _factoryRule.ExtractFactory(@class);

			var dependency = _dependencyRule.Extract(factory);
			var namespaces = _usingRule.Extract(factory, dependency);
			var disposableInfo = DisposableInfo.Aggregate(factory, dependency);
			return new Generation(namespaces, factory, dependency, disposableInfo);
		}
	}
}
