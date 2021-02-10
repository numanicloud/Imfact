using Deptorygen2.Core.Aggregation;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Semanticses;
using Deptorygen2.Core.Steps.Api;
using Deptorygen2.Core.SyntaxBuilding;
using Deptorygen2.Core.Writing;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Facade
{
	public class GenerationFacade
	{
		private readonly IAnalysisContext _context;
		private readonly AspectAggregator _aspectAggregator = new();
		private readonly SemanticsAggregator _semanticsAggregator;
		private readonly DefinitionAggregator _syntaxBuilder = new();
		private readonly SourceCodeBuilder _writer = new();

		public GenerationFacade(IAnalysisContext context)
		{
			_context = context;
			_semanticsAggregator = new SemanticsAggregator(context);
		}

		public SyntaxOnAspect? AspectStep(ClassDeclarationSyntax syntax)
		{
			return _aspectAggregator.Aggregate(syntax, _context) is {} aspect
				? new SyntaxOnAspect(aspect) : null;
		}

		public DeptorygenSemantics? SemanticsStep(SyntaxOnAspect aspect)
		{
			return _semanticsAggregator.Aggregate(aspect.Class, _context) is { } semantics
				? new DeptorygenSemantics(semantics) : null;
		}

		public DeptorygenDefinition DefinitionStep(DeptorygenSemantics semantics)
		{
			var definition = _syntaxBuilder.Encode(semantics.Semantics);
			return new DeptorygenDefinition(definition);
		}

		public SourceFile ConvertToSourceCode(DeptorygenDefinition definition)
		{
			return _writer.Build(definition.Definition);
		}
	}
}
