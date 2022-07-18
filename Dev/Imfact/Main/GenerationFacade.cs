using System.Linq;
using Imfact.Steps.Dependency.Components;
using Imfact.Steps.Ranking;
using Imfact.Steps.Semanticses;
using Imfact.Steps.Writing;
using Imfact.Utilities;

namespace Imfact.Main
{
	internal class GenerationFacade
	{
		// MEMO: GenerationContextはコンパイルごとに異なる。IAnalysisContextはソースファイルごとに異なる
		private readonly GenerationContext _genContext = new ();
		private readonly StepFactory _stepFactory = new();
		private readonly SemanticsStep _semanticsStep;

		public GenerationFacade()
		{
			_semanticsStep = _stepFactory.Semantics();
		}

		public SourceFile[] Run(CandidateClass[] candidates)
		{
			var ranking = new RankingStep();
			var ranked = ranking.Run(candidates);
			var factoryDependency = new FactoryDependencyContext();

			return ranked.OrderBy(x => x.Rank)
				.Select(x => RunGeneration(x, factoryDependency))
				.FilterNull()
				.ToArray();
		}

		private SourceFile? RunGeneration(RankedClass syntax, FactoryDependencyContext factoryDependency)
		{
			return _stepFactory.Aspect(_genContext, syntax.Context).Run(syntax)
				.Then(aspect => _semanticsStep.Run(aspect))
				.Then(semantics => _stepFactory.Dependency(semantics, factoryDependency).Run())
				.Then(dependency =>
				{
					var result = _stepFactory.Definition(dependency).Build();

					var ctor = result.ConstructorRecord;
					_genContext.Constructors[ctor.ClassType.Id] = ctor;

					return result;
				})
				.Then(definition => _stepFactory.Writing(definition).Write());
		}
	}
}
