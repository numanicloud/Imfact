using System;
using System.Collections.Generic;
using System.Text;
using Imfact.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Imfact
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

			var csCompilation = (CSharpCompilation) context.Compilation;
			var options = csCompilation.SyntaxTrees.Length > 0
				? (CSharpParseOptions) csCompilation.SyntaxTrees[0].Options
				: CSharpParseOptions.Default;
			var compilation =
				context.Compilation.AddSyntaxTrees(AnnotationGenerator.GetSyntaxTrees(options));

			if (context.SyntaxReceiver is not FactorySyntaxReceiver receiver
			    || receiver.SyntaxTree is null)
			{
				return;
			}

			var semanticModel = compilation.GetSemanticModel(receiver.SyntaxTree);
			var facade = new GenerationFacade(semanticModel);

			var sourceFiles = facade.Run(receiver.CandidateClasses.ToArray());
			foreach (var file in sourceFiles)
			{
				var sourceText = SourceText.From(file.Contents, Encoding.UTF8);
				context.AddSource(file.FileName, sourceText);
			}
		}

		private Exception GetException(Exception ex)
		{
			return new AggregateException($"Exception in source generator. {ex.Message} {ex.StackTrace}", ex);
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
