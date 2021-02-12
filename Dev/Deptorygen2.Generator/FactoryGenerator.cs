using System.Collections.Generic;
using System.Text;
using Deptorygen2.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Deptorygen2.Generator
{
	[Generator]
	public class FactoryGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new FactorySyntaxReceiver());
		}

		public void Execute(GeneratorExecutionContext context)
		{
			System.Diagnostics.Debugger.Launch();
			AnnotationGenerator.AddSource(in context);

			if (context.SyntaxReceiver is not FactorySyntaxReceiver receiver
				|| receiver.SyntaxTree is null)
			{
				return;
			}

			var semanticModel = context.Compilation.GetSemanticModel(receiver.SyntaxTree);
			var facade = new GenerationFacade(semanticModel);

			foreach (var candidateClass in receiver.CandidateClasses)
			{
				if (facade.RunGeneration(candidateClass) is {} sourceFile)
				{
					var sourceText = SourceText.From(sourceFile.Contents, Encoding.UTF8);
					context.AddSource(sourceFile.FileName, sourceText);
				}
			}
		}
	}

	class FactorySyntaxReceiver : ISyntaxReceiver
	{
		public SyntaxTree? SyntaxTree { get; set; }
		public List<ClassDeclarationSyntax> CandidateClasses { get; } = new();

		public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
		{
			SyntaxTree ??= syntaxNode.SyntaxTree;

			if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax
				&& classDeclarationSyntax.AttributeLists.Count > 0)
			{
				CandidateClasses.Add(classDeclarationSyntax);
			}
		}
	}
}
