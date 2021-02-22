using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Creation.Strategies.Template;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Expressions.Strategies
{
	internal class FactoryExpressionStrategy<TFactory, TResolver> : IExpressionStrategy
		where TFactory : IFactorySemantics
		where TResolver : IResolverSemantics
	{
		private readonly IFactorySource<TFactory> _factorySource;
		private readonly Dictionary<TypeRecord, Source[]> _map;

		public FactoryExpressionStrategy(IFactorySource<TFactory> factorySource,
			IResolverSource<TResolver> resolverSource)
		{
			_factorySource = factorySource;

			var grouped = from factory in _factorySource.GetDelegationSource()
						  from resolver in resolverSource.GetResolverSource(factory)
						  select new Source(factory, resolver) into source
						  group source by source.Resolver.ReturnType.Record;
			_map = grouped.ToDictionary(x => x.Key, x => x.ToArray());
		}

		public ICreationNode? GetExpression(CreationContext context)
		{
			// TODO: Resolverの等値性判定。

			if (_map.GetValueOrDefault(context.TypeToResolve[0].Record) is not {} resolutions
				|| Filter(resolutions, context.Caller).FirstOrDefault() is not { } source)
			{
				return null;
			}

			var variable = _factorySource.GetVariableName(source.Factory);
			return new Invocation($"{variable}.{source.Resolver.MethodName}",
				GetArguments(source, context));
		}

		protected virtual Source[] Filter(Source[] source, IResolverSemantics caller)
		{
			return source;
		}

		private ICreationNode[] GetArguments(Source source, CreationContext context)
		{
			var resolver = source.Resolver;
			var childContext = context with
			{
				TypeToResolve = resolver.Parameters.Select(x => x.Type).ToArray()
			};

			return context.Injector.GetExpression(childContext).ToArray();
		}

		internal record Source(TFactory Factory, TResolver Resolver);
	}
}
