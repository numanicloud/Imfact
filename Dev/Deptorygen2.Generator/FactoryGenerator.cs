using System.Collections.Generic;
using System.Text;
using Deptorygen2.Core.Facade;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

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
			if (context.SyntaxReceiver is not FactorySyntaxReceiver receiver
				|| receiver.SyntaxTree is null)
			{
				return;
			}

			var semanticModel = context.Compilation.GetSemanticModel(receiver.SyntaxTree);

			var facade = new GenerationFacade();
			foreach (var candidateClass in receiver.CandidateClasses)
			{

				var semantics = await facade.SemanticsStep(candidateClass);
				var definition = facade.DefinitionStep(semantics);
				var sourceFile = facade.ConvertToSourceCode(definition);

				var sourceText = SourceText.From(sourceFile.Contents, Encoding.UTF8);
				context.AddSource(sourceFile.FileName, sourceText);
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

	internal class CompilationAnalysisContext : IAnalysisContext
	{
		private readonly SemanticModel _semanticModel;

		public CompilationAnalysisContext(SemanticModel semanticModel)
		{
			_semanticModel = semanticModel;
		}

		public ITypeSymbol? GeTypeSymbol(TypeSyntax syntax)
		{
			return _semanticModel.GetSymbolInfo(syntax).Symbol is ITypeSymbol type ? type
				: null;
		}

		public IMethodSymbol? GetMethodSymbol(MethodDeclarationSyntax syntax)
		{
			return _semanticModel.GetDeclaredSymbol(syntax) is IMethodSymbol method ? method
					: null;
		}

		public IPropertySymbol? GetPropertySymbol(PropertyDeclarationSyntax syntax)
		{
			return _semanticModel.GetDeclaredSymbol(syntax) is IPropertySymbol prop ? prop
				: null;
		}

		public INamedTypeSymbol? GetNamedTypeSymbol(TypeDeclarationSyntax syntax)
		{
			return _semanticModel.GetDeclaredSymbol(syntax) is INamedTypeSymbol type
				? type
				: null;
		}
	}
}
