using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps;
using Deptorygen2.Core.Steps.Aspects;
using Deptorygen2.Core.Steps.Definitions;
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
		private readonly ClassRule _classAggregator;
		private readonly SemanticsRule _semanticsAggregator;

		public GenerationFacade(SemanticModel semanticModel)
		{
			_context = new CompilationAnalysisContext(semanticModel);
			_semanticsAggregator = new SemanticsRule();
			_classAggregator = new ClassRule(_context);
		}

		public SourceFile[] Run(ClassDeclarationSyntax[] syntaxes)
		{
			var ranking = new RankFilter();
			var ranked = ranking.FilterClasses(syntaxes, _context);

			return ranked.OrderBy(x => x.Rank)
				.Select(RunGeneration)
				.FilterNull()
				.ToArray();
		}

		private SourceFile? RunGeneration(RankedClass syntax)
		{
			return AspectStep(syntax)
				.Then(SemanticsStep)
				.Then(DefinitionStep)
				.Then(SourceCodeStep);
		}

		private SyntaxOnAspect AspectStep(RankedClass syntax)
		{
			var aspect = _classAggregator.Aggregate(syntax);
			return new SyntaxOnAspect(aspect);
		}

		private DeptorygenSemantics? SemanticsStep(SyntaxOnAspect aspect)
		{
			return _semanticsAggregator.Aggregate(aspect.Class) is { } semantics
				? new DeptorygenSemantics(semantics) : null;
		}

		private SourceTreeDefinition DefinitionStep(DeptorygenSemantics semantics)
		{
			var builder = new DefinitionTreeBuilder(semantics.Semantics);
			return builder.Build();
		}

		private SourceFile SourceCodeStep(SourceTreeDefinition definition)
		{
			var writer = new SourceCodeBuilder(definition);
			return writer.Write();
		}
	}
}
