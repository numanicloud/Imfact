using Imfact.Steps.Dependency.Interfaces;
using Imfact.Steps.Dependency.Strategies;
using Imfact.Steps.Semanticses.Interfaces;

namespace Imfact.Main;

static class FactorySourceExtensions
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