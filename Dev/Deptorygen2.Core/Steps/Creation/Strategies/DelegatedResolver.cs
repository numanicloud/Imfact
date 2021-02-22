using Deptorygen2.Core.Steps.Creation.Abstraction;
using Deptorygen2.Core.Steps.Creation.Strategies.Template;
using Deptorygen2.Core.Steps.Semanticses.Nodes;

namespace Deptorygen2.Core.Steps.Creation.Strategies
{
	internal class DelegatedResolver : ICreationStrategy
	{
		private readonly TemplateStrategy<Delegation, Semanticses.Nodes.Resolver> _template;

		public DelegatedResolver(Generation semantics)
		{
			_template = new TemplateStrategy<Delegation, Semanticses.Nodes.Resolver>(
				new DelegationSource(),
				new ResolverSource(),
				semantics);
		}

		public string? GetCode(CreationRequest request, ICreationAggregator aggregator)
		{
			return _template.GetCode(request, aggregator);
		}
	}
}
