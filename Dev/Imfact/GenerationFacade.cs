using System.Linq;
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

		private AspectResult AspectStep(RankedClass syntax)
		{
			var aspect = _classAggregator.Run(syntax);
			return new AspectResult(aspect);
		}

		private SemanticsResult SemanticsStep(AspectResult aspect)
		{
			return _semanticsAggregator.Run(aspect.Class);
		}

		private DependencyResult ResolutionStep(SemanticsResult semantics)
		{
			var builder = new DependencyStep(semantics);
			return builder.Run();
		}

		private DefinitionResult DefinitionStep(DependencyResult dependency)
		{
			var builder = new DefinitionStep(dependency);
			return builder.Build();
		}

		private SourceFile SourceCodeStep(DefinitionResult definitionStepResult)
		{
			var writer = new WritingStep(definitionStepResult);
			return writer.Write();
		}
	}
}
