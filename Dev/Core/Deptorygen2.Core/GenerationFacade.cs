using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps;
using Deptorygen2.Core.Steps.Aspects;
using Deptorygen2.Core.Steps.Definitions;
using Deptorygen2.Core.Steps.Expressions;
using Deptorygen2.Core.Steps.Ranking;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Steps.Semanticses.Rules;
using Deptorygen2.Core.Steps.Writing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Deptorygen2.Core.Utilities;
using NacHelpers.Extensions;

namespace Deptorygen2.Core
{
	public class GenerationFacade
	{
		private readonly IAnalysisContext _context;
		private readonly AspectStep _classAggregator;
		private readonly SemanticsStep _semanticsAggregator;

		public GenerationFacade(SemanticModel semanticModel)
		{
			_context = new CompilationAnalysisContext(semanticModel);
			_classAggregator = new AspectStep(_context);
			_semanticsAggregator = new SemanticsStep();
		}

		public SourceFile[] Run(ClassDeclarationSyntax[] syntaxes)
		{
			var ranking = new RankingStep();
			var ranked = ranking.Run(syntaxes, _context);

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
					_context.Constructors[ctor.ClassType.Record] = ctor;

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
