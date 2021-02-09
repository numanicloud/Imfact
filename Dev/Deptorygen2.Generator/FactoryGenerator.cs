using System.Collections.Generic;
using System.Text;
using Deptorygen2.Core.Facade;
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

		public async void Execute(GeneratorExecutionContext context)
		{
			if (context.SyntaxReceiver is not FactorySyntaxReceiver receiver)
			{
				return;
			}

			var facade = new GenerationFacade();
			foreach (var candidateClass in receiver.CandidateClasses)
			{
				var semantics = await facade.ConvertToSemanticsAsync(candidateClass);
				var definition = facade.ConvertToDefinition(semantics);
				var sourceFile = facade.ConvertToSourceCode(definition);

				var sourceText = SourceText.From(sourceFile.Contents, Encoding.UTF8);
				context.AddSource(sourceFile.FileName, sourceText);
			}
		}
	}

	class FactorySyntaxReceiver : ISyntaxReceiver
	{
		public List<ClassDeclarationSyntax> CandidateClasses { get; } = new();

		public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
		{
			if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax
				&& classDeclarationSyntax.AttributeLists.Count > 0)
			{
				CandidateClasses.Add(classDeclarationSyntax);
			}
		}
	}
}
