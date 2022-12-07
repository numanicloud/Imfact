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
    private readonly GenerationContext _genContext = new();
    private readonly StepFactory _stepFactory = new();
    private readonly SemanticsStep _semanticsStep;

    public GenerationFacade()
    {
        _semanticsStep = _stepFactory.Semantics();
    }

    public SourceFile[] Run(FactoryCandidate[] candidates)
    {
        if (candidates.Length != 1)
        {
            throw new ArgumentException(nameof(candidates));
        }
        return new RankedClass[] { new(candidates[0], null, 0) }
            .Select(x => RunGeneration(x, candidates))
            .FilterNull()
            .ToArray();


        var ranking = new RankingStep();
        var ranked = ranking.Run(candidates);

        return ranked.OrderBy(x => x.Rank)
            .Select(x => RunGeneration(x, candidates))
            .FilterNull()
            .ToArray();
    }

    private SourceFile? RunGeneration(RankedClass syntax, FactoryCandidate[] factoryCandidates)
    {
        return _stepFactory.Aspect(_genContext, factoryCandidates)
            .Run(syntax)
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