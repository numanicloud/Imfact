using System.Collections.Generic;
using Imfact.Interfaces;
using Imfact.Steps.Aspects;
using Imfact.Steps.Aspects.Rules;
using Imfact.Steps.Definitions;
using Imfact.Steps.Definitions.Builders;
using Imfact.Steps.Dependency;
using Imfact.Steps.Dependency.Components;
using Imfact.Steps.Dependency.Interfaces;
using Imfact.Steps.Dependency.Strategies;
using Imfact.Steps.Semanticses;
using Imfact.Steps.Semanticses.Rules;
using Imfact.Steps.Writing;

namespace Imfact.Main
{
	internal sealed class StepFactory
	{
		public AspectStep Aspect(GenerationContext genContext, IAnalysisContext context)
		{
			var typeRule = new TypeRule();

			var methodRule = new MethodRule(context, new AttributeRule(typeRule), typeRule);

			var classRule = new ClassRule(genContext,
				methodRule,
				new PropertyRule(context, methodRule));

			return new AspectStep(classRule);
		}

		public SemanticsStep Semantics()
		{
			return new SemanticsStep(new FactoryRule(new ResolverRule()));
		}

		public DefinitionStep Definition(DependencyResult dependency)
		{
			var methodService = new MethodService(dependency);
			var builder = new ClassBuilder(dependency.Semantics, dependency,
				new MethodBuilder(
					methodService,
					dependency.Injection,
					new DisposeMethodBuilder(dependency),
					new ConstructorBuilder(dependency, methodService)));
			return new DefinitionStep(dependency, builder);
		}

		public DependencyStep Dependency(SemanticsResult semantics)
		{
			var crawler = new CreationCrawler(GetCreations(semantics));
			return new DependencyStep(semantics, crawler);
		}

		public WritingStep Writing(DefinitionResult definition)
		{
			return new WritingStep(definition);
		}

		private static IEnumerable<IExpressionStrategy> GetCreations(SemanticsResult semantics)
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
}
