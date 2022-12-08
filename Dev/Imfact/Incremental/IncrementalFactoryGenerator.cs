using System.Diagnostics;
using System.Threading;
using Imfact.Annotations;
using Imfact.Main;
using Imfact.Steps.Writing;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Imfact.Incremental;

[Generator]
public class IncrementalFactoryGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
#if DEBUG
        //DebugHelper.Attach();
#endif

        context.RegisterPostInitializationOutput(GenerateInitialCode);

        IncrementalValueProvider<AnnotationContext> annotations =
            context.CompilationProvider
                .Select(AnnotationContext.FromCompilation);

        // Collect メソッドを使って、全てのノードを総合的に評価することができそう
        IncrementalValuesProvider<FactoryCandidate> candidates =
            context.SyntaxProvider
                .CreateSyntaxProvider(Predicate, Transform)
                .Combine(annotations)
                .Select(PostTransform)
                .Where(x => x != null)!;

        context.RegisterSourceOutput(candidates, GenerateFileEmbed);
    }

    private void GenerateInitialCode(IncrementalGeneratorPostInitializationContext context)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
        context.AddSource(
            hintName: $"Annotations.g.cs",
            source: AnnotationDefinitions.BuildAnnotationCode());
    }

    private bool Predicate(SyntaxNode node, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return node is TypeDeclarationSyntax { AttributeLists.Count: > 0 } syntax
            && node is not InterfaceDeclarationSyntax
            && syntax.Modifiers.IndexOf(SyntaxKind.PartialKeyword) != -1;
    }

    private FactoryIncremental? Transform(GeneratorSyntaxContext context, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var syntax = (context.Node as TypeDeclarationSyntax)!;
        var symbol = context.SemanticModel.GetDeclaredSymbol(syntax, ct);
        var methods = syntax.Members
            .OfType<MethodDeclarationSyntax>()
            .Select(GetResolver)
            .FilterNull()
            .ToArray();

        return symbol is not null
            ? new FactoryIncremental(symbol, methods)
            : null;

        ResolverIncremental? GetResolver(MethodDeclarationSyntax method)
        {
            ct.ThrowIfCancellationRequested();
            return context.SemanticModel.GetDeclaredSymbol(method) is { } ms
                ? new ResolverIncremental(ms, GeneralRule.Instance.IsResolverToGenerate(method))
                : null;
        }
    }

    private FactoryCandidate? PostTransform(
        (FactoryIncremental? factory, AnnotationContext annotations) tuple,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        if (tuple.factory is not { } factory) return null;
        if (!GeneralRule.Instance.IsFactoryCandidate(factory, tuple.annotations)) return null;

        return new FactoryCandidate(factory.Symbol,
            factory.Methods
                .Select(x => new ResolverCandidate(x.Symbol, x.IsToGenerate))
                .ToArray(),
            tuple.annotations);
    }

    private void GenerateFileEmbed(SourceProductionContext context, FactoryCandidate candidate)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        var facade = new GenerationFacade();

        SourceFile[] sources;
        try
        {
            sources = facade.Run(new[] { candidate });
        }
        catch (Exception ex)
        {
            Debugger.Break();
            context.ReportDiagnostic(DebugHelper.Error(
                "IMF001",
                "Internal error",
                $"Internal error occurd in Imfact: {ex}"));
            return;
        }

        context.AddSource(
            hintName: $"{candidate.Symbol.Name}.g.cs",
            source: sources[0].Contents);
    }
}


