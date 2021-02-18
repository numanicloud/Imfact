using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Aspects.Nodes;
using Deptorygen2.Core.Steps.Semanticses.Nodes;

namespace Deptorygen2.Core.Steps.Semanticses
{
	internal sealed class SemanticsRule
	{
		private readonly FactoryRule _factoryRule;
		private readonly UsingRule _usingRule = new();
		private readonly DependencyRule _dependencyRule = new();

		public SemanticsRule(IAnalysisContext context)
		{
			_factoryRule = new FactoryRule(context, new ResolverRule(context));
		}

		public Generation? Aggregate(Class @class)
		{
			var factory = _factoryRule.ExtractFactory(@class);
			if (factory is null)
			{
				return null;
			}

			var dependency = _dependencyRule.Extract(factory);
			var namespaces = _usingRule.Extract(factory, dependency);
			var disposableInfo = DisposableInfo.Aggregate(factory, dependency);
			return new Generation(namespaces, factory, dependency, disposableInfo);
		}
	}
}
