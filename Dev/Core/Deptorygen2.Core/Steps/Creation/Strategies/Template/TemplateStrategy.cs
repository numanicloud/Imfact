using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Creation.Abstraction;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Steps.Semanticses.Nodes;

namespace Deptorygen2.Core.Steps.Creation.Strategies.Template
{
	internal class TemplateStrategy<TFactory, TResolver> : CreationMethodBase<TemplateStrategy<TFactory, TResolver>.Source>
		where TFactory : IFactorySemantics
		where TResolver : IResolverSemantics
	{
		private readonly IFactorySource<TFactory> _factorySource;
		private readonly IResolverSource<TResolver> _resolverSource;

		public TemplateStrategy(IFactorySource<TFactory> factorySource,
			IResolverSource<TResolver> resolverSource,
			SemanticsRoot semantics) : base(semantics)
		{
			_factorySource = factorySource;
			_resolverSource = resolverSource;
		}

		protected override string GetCreationCode(Source resolution, GivenParameter[] given, ICreationAggregator aggregator)
		{
			var variable = _factorySource.GetVariableName(resolution.Factory);
			var invocation = MethodInvocation(resolution.Resolver, given, aggregator);
			return $"{variable}.{invocation}";
		}

		protected override IEnumerable<Source> GetSource(SemanticsRoot semantics)
		{
			return from delegation in _factorySource.GetDelegationSource()
				   from resolver in _resolverSource.GetResolverSource(delegation)
				   select new Source(delegation, resolver);
		}

		protected override TypeRecord GetTypeInfo(Source source)
		{
			return source.Resolver.ReturnType.Record;
		}

		private string MethodInvocation(IResolverSemantics resolver,
			GivenParameter[] given,
			ICreationAggregator injector)
		{
			var request = new MultipleCreationRequest(
				resolver.Parameters.Select(x => x.Type).ToArray(), given, false);

			return $"{resolver.MethodName}({GetArgList(request, injector)})";
		}

		public record Source(TFactory Factory, TResolver Resolver);
	}
}
