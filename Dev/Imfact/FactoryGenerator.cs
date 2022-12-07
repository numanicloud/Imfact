using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Imfact.Annotations;
using Imfact.Entities;
using Imfact.Incremental;
using Imfact.Main;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Imfact;

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
            
			var candidates = ExtractCandidates(receiver.CandidateClasses, compilation);
            var facade = new GenerationFacade();

            var sourceFiles = facade.Run(candidates);
            foreach (var file in sourceFiles)
            {
                using var profiler = TimeProfiler.Create("File-Adding");
                var sourceText = SourceText.From(file.Contents, Encoding.UTF8);
                context.AddSource(file.FileName, sourceText);
            }
        }
        catch (System.Exception ex)
        {
            var title = "Internal exception occurred within Imfact source generator.";
            var noPhantom = string.IsNullOrWhiteSpace(ex.StackTrace) ? "<empty>" : ex.StackTrace;
            var fullDesc = $"{ex} {noPhantom}";

            fullDesc = fullDesc.Replace("\n", "").Replace("\r", "");

            var diagnostic = Diagnostic.Create(
                new DiagnosticDescriptor("IMF0002",
                    $"{title} {fullDesc} v19",
                    $"{title} {fullDesc} v19",
                    "Internal",
                    DiagnosticSeverity.Warning,
                    true,
                    ex.ToString(),
                    null,
                    WellKnownDiagnosticTags.AnalyzerException),
                null, (object[]?)null);
            context.ReportDiagnostic(diagnostic);
            Debug.WriteLine($"{title} {fullDesc}");
        }
    }

    private FactoryCandidate[] ExtractCandidates(
        IEnumerable<ClassDeclarationSyntax> syntaxes,
        Compilation compilation)
    {
        return syntaxes
			.Where(x => x.AttributeLists.HasAttribute(new AttributeName(nameof(FactoryAttribute))))
			.Where(x => x.Modifiers.IndexOf(SyntaxKind.PartialKeyword) != -1)
			.Select(x =>
			{
				var semanticModel = compilation.GetSemanticModel(x.SyntaxTree);
				return semanticModel.GetDeclaredSymbol(x) is { } symbol
					? new FactoryCandidate(symbol, GetResolvers(x, semanticModel))
					: null;
			}).FilterNull()
            .ToArray();

        ResolverCandidate[] GetResolvers(ClassDeclarationSyntax @class,
			SemanticModel semanticModel)
		{
			return @class.Members
				.OfType<MethodDeclarationSyntax>()
				.Select(x => semanticModel.GetDeclaredSymbol(x) is { } symbol
					? new ResolverCandidate(symbol, IsValidResolverModifier(x))
					: null)
				.FilterNull()
				.ToArray();
		}

		bool IsValidResolverModifier(MethodDeclarationSyntax method)
		{
			return method.Modifiers.IndexOf(SyntaxKind.PartialKeyword) != -1
				&& method.Modifiers.Any(x =>
					x.IsKind(SyntaxKind.PublicKeyword)
					|| x.IsKind(SyntaxKind.PrivateKeyword)
					|| x.IsKind(SyntaxKind.InternalKeyword)
					|| x.IsKind(SyntaxKind.ProtectedKeyword));
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