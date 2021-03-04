using System.Linq;
using Imfact.Steps;
using Imfact.Steps.Aspects;
using Imfact.Steps.Definitions;
using Imfact.Steps.Dependency;
using Imfact.Steps.Ranking;
using Imfact.Steps.Semanticses;
using Imfact.Steps.Writing;
using Imfact.Utilities;

namespace Imfact
{
	internal class GenerationFacade
	{
		private readonly AspectStep _classAggregator;
		private readonly SemanticsStep _semanticsAggregator;
		private readonly GenerationContext _genContext = new ();

		public GenerationFacade()
		{
			_classAggregator = new AspectStep(_genContext);
			_semanticsAggregator = new SemanticsStep();
		}

		public SourceFile[] Run(CandidateClass[] candidates)
		{
			var ranking = new RankingStep();
			var ranked = ranking.Run(candidates);

			return ranked.OrderBy(x => x.Rank)
				.Select(RunGeneration)
				.FilterNull()
				.ToArray();
		}

		private SourceFile? RunGeneration(RankedClass syntax)
		{
			return AspectStep(syntax)
				.Then(SemanticsStep)
				.Then(ResolutionStep)
				.Then(semantics =>
				{
					var result = DefinitionStep(semantics);

					var ctor = result.ConstructorRecord;
					_genContext.Constructors[ctor.ClassType.Id] = ctor;

					return result;
				})
				.Then(SourceCodeStep);
		}

		private SyntaxOnAspect AspectStep(RankedClass syntax)
		{
			var aspect = _classAggregator.Run(syntax);
			return new SyntaxOnAspect(aspect);
		}

		private SemanticsRoot SemanticsStep(SyntaxOnAspect aspect)
		{
			return _semanticsAggregator.Run(aspect.Class);
		}

		private DependencyRoot ResolutionStep(SemanticsRoot semantics)
		{
			var builder = new DependencyStep(semantics);
			return builder.Run();
		}

		private DefinitionStepResult DefinitionStep(DependencyRoot dependency)
		{
			var builder = new DefinitionStep(dependency);
			return builder.Build();
		}

		private SourceFile SourceCodeStep(DefinitionStepResult definitionStepResult)
		{
			var writer = new SourceCodeBuilder(definitionStepResult);
			return writer.Write();
		}
	}
}
