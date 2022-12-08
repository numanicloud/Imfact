using System;
using System.Linq;
using Imfact.Incremental;
using Imfact.Steps.Ranking;
using Imfact.Steps.Semanticses;
using Imfact.Steps.Writing;
using Imfact.Utilities;

namespace Imfact.Main;

internal class GenerationFacade
{
    // MEMO: GenerationContextはコンパイルごとに異なる。IAnalysisContextはソースファイルごとに異なる
    private readonly GenerationContext _genContext;
    private readonly StepFactory _stepFactory;
    private readonly SemanticsStep _semanticsStep;

    public GenerationFacade(Logger logger)
    {
        _genContext = new GenerationContext()
        {
            Logger = logger
        };
		_stepFactory = new StepFactory(_genContext);
        _semanticsStep = _stepFactory.Semantics();
    }

    public SourceFile[] Run(GenerationSource generationSource)
    {
        var ranking = new RankingStep();
        var ranked = ranking.Run(generationSource.Factories);

        var result = ranked.OrderBy(x => x.Rank)
            .Select(x => RunGeneration(x, generationSource.Annotations))
            .FilterNull()
            .ToArray();

        _genContext.Profiler.WriteStats(_genContext.Logger);

		return result;
	}

    private SourceFile? RunGeneration(RankedClass ranked, AnnotationContext annotations)
    {
        return _stepFactory.Aspect(annotations)
            .Run(ranked)
            .Then(aspect => _semanticsStep.Run(aspect))
            .Then(semantics => _stepFactory.Dependency(semantics, _genContext).Run())
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