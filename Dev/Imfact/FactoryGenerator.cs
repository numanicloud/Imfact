using System.Collections.Generic;
using System.Linq;
using System.Text;
using Imfact.Annotations;
using Imfact.Main;
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
			try
			{
				//System.Diagnostics.Debugger.Launch();
				AnnotationGenerator.AddSource(in context);

				if (context.SyntaxReceiver is not FactorySyntaxReceiver receiver
					|| receiver.SyntaxTree is null)
				{
					return;
				}

				var csCompilation = (CSharpCompilation)context.Compilation;
				var options = (CSharpParseOptions)csCompilation.SyntaxTrees[0].Options;
				var compilation =
					context.Compilation.AddSyntaxTrees(AnnotationGenerator.GetSyntaxTrees(options));

				var candidates = receiver.CandidateClasses
					.Select(x =>
					{
						var sm = compilation.GetSemanticModel(x.SyntaxTree);
						return new CandidateClass(x, new CompilationAnalysisContext(sm));
					})
					.ToArray();
				var facade = new GenerationFacade();

				var sourceFiles = facade.Run(candidates);
				foreach (var file in sourceFiles)
				{
					var sourceText = SourceText.From(file.Contents, Encoding.UTF8);
					context.AddSource(file.FileName, sourceText);
				}
			}
			catch (System.Exception ex)
			{
				var title = $"Internal exception occurred within Imfact source generator. {ex.Message}";

				var diagnostic = Diagnostic.Create(
					new DiagnosticDescriptor("IMF0001", title, title, "Internal", DiagnosticSeverity.Warning, true, ex.ToString()),
					null, (object[]?)null);
				context.ReportDiagnostic(diagnostic);
				throw;
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
