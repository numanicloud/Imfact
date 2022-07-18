using System.Collections.Generic;
using System.Linq;
using Imfact.Entities;
using Imfact.Steps.Dependency.Interfaces;
using Imfact.Steps.Semanticses.Interfaces;
using Imfact.Steps.Semanticses.Records;

namespace Imfact.Steps.Dependency.Components
{
	internal class CreationCrawler
	{
		private readonly FactoryDependencyContext _factoryDependency;
		private readonly IExpressionStrategy[] _strategies;

		public CreationCrawler(IEnumerable<IExpressionStrategy> strategies,
			FactoryDependencyContext factoryDependency)
		{
			_factoryDependency = factoryDependency;
			_strategies = strategies.ToArray();
		}

		public IEnumerable<ICreationNode> GetExpression(CreationContext context)
		{
			while (context.TypeToResolve.Any())
			{
				var type = context.TypeToResolve[0];

				yield return _strategies.Select(x => x.GetExpression(context))
					.FirstOrDefault(x => x is not null)
					?? _factoryDependency.RegisterUnsatisfied(type, context);

				context = context with
				{
					TypeToResolve = context.TypeToResolve.Skip(1).ToArray()
				};
			}
		}
	}

	// 型を1つ解決すると、TypeToResolveの中身が減る。中身が空になったら解決完了。
	// パラメータでもって解決すると、それはConsumedParametersに記憶される。以後同じパラメータは使われない。
	internal record CreationContext(
		TypeAnalysis FactoryType,
		IResolverSemantics Caller,
		TypeAnalysis[] TypeToResolve,
		List<Parameter> ConsumedParameters,
		CreationCrawler Injector);
}
