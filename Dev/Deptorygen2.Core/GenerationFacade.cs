using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps;
using Deptorygen2.Core.Steps.Aspects;
using Deptorygen2.Core.Steps.Definitions;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Steps.Semanticses.Rules;
using Deptorygen2.Core.Steps.Writing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core
{
	public class GenerationFacade
	{
		private readonly IAnalysisContext _context;
		private readonly AspectRule _aspectAggregator;
		private readonly SemanticsRule _semanticsAggregator;

		public GenerationFacade(SemanticModel semanticModel)
		{
			_context = new CompilationAnalysisContext(semanticModel);
			_semanticsAggregator = new SemanticsRule();
			_aspectAggregator = new AspectRule(_context);
		}

		public SourceFile? RunGeneration(ClassDeclarationSyntax syntax)
		{
			return AspectStep(syntax)
				.Then(SemanticsStep)
				.Then(DefinitionStep)
				.Then(SourceCodeStep);
		}

		private SyntaxOnAspect? AspectStep(ClassDeclarationSyntax syntax)
		{
			return _aspectAggregator.Aggregate(syntax) is { } aspect
				? new SyntaxOnAspect(aspect) : null;
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
