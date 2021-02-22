using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Steps.Expressions;
using Deptorygen2.Core.Steps.Expressions.Strategies;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Dependency.Components
{
	internal class CreationCrawler
	{
		private readonly IExpressionStrategy[] _strategies;

		public CreationCrawler(SemanticsRoot semantics)
		{
			_strategies = GetCreations(semantics).ToArray();
		}

		public IEnumerable<ICreationNode> GetExpression(CreationContext context)
		{
			while (context.TypeToResolve.Any())
			{
				var type = context.TypeToResolve[0];

				yield return _strategies.Select(x => x.GetExpression(context))
					             .FirstOrDefault(x => x is not null)
				             ?? new UnsatisfiedField(type, ToFieldName(type));

				context = context with
				{
					TypeToResolve = context.TypeToResolve.Skip(1).ToArray()
				};
			}

			static string ToFieldName(TypeNode type)
			{
				return "_" + type.Name.ToLowerCamelCase();
			}
		}

		private static IEnumerable<IExpressionStrategy> GetCreations(SemanticsRoot semantics)
		{
			var factory = new RootFactorySource(semantics);
			var delegation = new DelegationSource(semantics);
			var inheritance = new InheritanceSource(semantics);
			var resolver = new ResolverSource();
			var multiResolver = new MultiResolverSource();

			// この順で評価されて、最初にマッチした解決方法が使われる
			yield return new ParameterStrategy();
			yield return factory.GetStrategyExp();
			yield return delegation.GetStrategyExp();
			yield return (delegation, resolver).GetStrategyExp();
			yield return (delegation, multiResolver).GetStrategyExp();
			yield return new RootResolverStrategy(factory, resolver);
			yield return (factory, multiResolver).GetStrategyExp();
			yield return (inheritance, resolver).GetStrategyExp();
			yield return (inheritance, multiResolver).GetStrategyExp();
			yield return new ConstructorStrategy();
		}
	}

	// 型を1つ解決すると、TypeToResolveの中身が減る。中身が空になったら解決完了。
	// パラメータでもって解決すると、それはConsumedParametersに記憶される。以後同じパラメータは使われない。
	internal record CreationContext(
		IResolverSemantics Caller,
		TypeNode[] TypeToResolve,
		List<Parameter> ConsumedParameters,
		CreationCrawler Injector);


	static class ExpressionExtensions
	{
		public static FactoryItselfStrategy<TFactory> GetStrategyExp<TFactory>(
			this IFactorySource<TFactory> source)
			where TFactory : IFactorySemantics
		{
			return new(source);
		}

		public static FactoryExpressionStrategy<TFactory, TResolver> GetStrategyExp<TFactory, TResolver>(
			this (IFactorySource<TFactory>, IResolverSource<TResolver>) components)
			where TFactory : IFactorySemantics
			where TResolver : IResolverSemantics
		{
			return new(components.Item1, components.Item2);
		}
	}
}
