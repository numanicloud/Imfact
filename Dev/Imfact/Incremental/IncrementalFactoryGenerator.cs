using System.Collections.Immutable;
using System.Diagnostics;
using Imfact.Annotations;
using Imfact.Main;
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

        IncrementalValueProvider<GenerationSource> generationSource =
            context.SyntaxProvider
                .CreateSyntaxProvider(Predicate, Transform)
                .Collect()
                .Combine(annotations)
                .Select(PostTransform);

        context.RegisterSourceOutput(generationSource, GenerateFileEmbed);
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
        return GeneralRule.Instance.IsFactoryClassDeclaration(node);
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

    private GenerationSource PostTransform(
        (ImmutableArray<FactoryIncremental?> factories, AnnotationContext annotations) tuple,
        CancellationToken ct)
    {
        var result = tuple.factories
            .Select(GetCandidate)
            .FilterNull()
            .ToArray();
        return new GenerationSource(result, tuple.annotations);

        FactoryCandidate? GetCandidate(FactoryIncremental? factory)
        {
            ct.ThrowIfCancellationRequested();

            if (factory is null) return null;
            if (!GeneralRule.Instance.IsFactoryCandidate(factory, tuple.annotations)) return null;

            return new FactoryCandidate(factory.Symbol,
                factory.Methods
                    .Select(x => new ResolverCandidate(x.Symbol, x.IsToGenerate))
                    .ToArray(),
                tuple.annotations);
        }
    }

    private void GenerateFileEmbed(SourceProductionContext context, GenerationSource source)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        var facade = new GenerationFacade(
            new Logger(msg => context.ReportDiagnostic(DebugHelper.Info(
                "IMF000",
                "Debug info",
                msg))));

        try
        {
            var generated = facade.Run(source);
			foreach (var file in generated)
            {
                context.AddSource(
                    hintName: file.FileName,
                    source: file.Contents);
            }
        }
        catch (Exception ex)
        {
#if DEBUG
			DebugHelper.Attach();
#endif
            context.ReportDiagnostic(DebugHelper.Error(
                "IMF001",
                "Internal error",
                $"Internal error occurd in Imfact: {ex}"));
        }
    }
}


