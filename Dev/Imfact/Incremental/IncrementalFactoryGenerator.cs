using System;
using System.Linq;
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
        DebugHelper.Attach();
#endif

        context.RegisterPostInitializationOutput(GenerateInitialCode);

        IncrementalValueProvider<AnnotationContext> annotations =
            context.CompilationProvider
                .Select(ExtractAnnotations);

        IncrementalValuesProvider<FactoryCandidate> candidates =
            context.SyntaxProvider
                .CreateSyntaxProvider(Predicate, Transform)
                .Combine(annotations)
                .Select(PostTransform)
                .Where(x => x != null)!;

        context.RegisterSourceOutput(candidates, GenerateFileEmbed);
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
            context.ReportDiagnostic(DebugHelper.Error(
                "IMF001",
                "Internal error",
                $"Internal error occurd in Imfact: {ex}"));
            throw;
        }

        context.AddSource(
            hintName: $"{candidate.Symbol.Name}.g.cs",
            source: sources[0].Contents);
    }

    private FactoryCandidate? PostTransform(
        (FactoryIncremental? factory, AnnotationContext annotations) tuple,
        CancellationToken ct)
    {
        try
        {
            ct.ThrowIfCancellationRequested();
            if (tuple.factory is null) return null;

            var attributes = tuple.factory.Symbol.GetAttributes();
            if (!attributes.Any(IsFactoryAttribute)) return null;

            return new FactoryCandidate(tuple.factory.Symbol,
                tuple.factory.Methods
                    .Select(x => new ResolverCandidate(x.Symbol, x.IsToGenerate))
                    .ToArray(),
                tuple.annotations);

            bool IsFactoryAttribute(AttributeData attributeData)
            {
                return SymbolEqualityComparer.Default
                    .Equals(attributeData.AttributeClass,
                        tuple.annotations.FactoryAttribute);
            }
        }
        catch (Exception ex)
        {
			throw;
        }
    }

    private FactoryIncremental? Transform(GeneratorSyntaxContext context, CancellationToken ct)
    {
        try
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
                    ? new ResolverIncremental(ms,
                        method.Modifiers.IndexOf(SyntaxKind.PartialKeyword) != -1
                        && method.Modifiers.Any(x =>
                            x.IsKind(SyntaxKind.PublicKeyword)
                            || x.IsKind(SyntaxKind.PrivateKeyword)
                            || x.IsKind(SyntaxKind.ProtectedKeyword)
                            || x.IsKind(SyntaxKind.InternalKeyword)))
                    : null;
            }
        }
        catch (Exception ex)
        {
			throw;
        }
    }

    private bool Predicate(SyntaxNode node, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return node is TypeDeclarationSyntax { AttributeLists.Count: > 0 } syntax
            && node is not InterfaceDeclarationSyntax
            && syntax.Modifiers.IndexOf(SyntaxKind.PartialKeyword) != -1;
    }

    private AnnotationContext ExtractAnnotations(Compilation compilation, CancellationToken ct)
    {
        var ns = AnnotationDefinitions.NamespaceName;
        return new AnnotationContext
        {
            FactoryAttribute = EnsureGetType(AnnotationDefinitions.FactoryAttributeName),
            ResolutionAttribute = EnsureGetType(AnnotationDefinitions.ResolutionAttributeName),
            HookAttribute = EnsureGetType(AnnotationDefinitions.HookAttributeName),
            ExporterAttribute = EnsureGetType(AnnotationDefinitions.ExporterAttributeName),
            CacheAttribute = EnsureGetType(AnnotationDefinitions.CacheAttributeName),
            CachePerResolutionAttribute = EnsureGetType(AnnotationDefinitions.CachePerResolutionAttributeName),
            TransientAttribute = EnsureGetType(AnnotationDefinitions.TransientAttributeName)
        };

        INamedTypeSymbol EnsureGetType(string typeName)
        {
            ct.ThrowIfCancellationRequested();
            return compilation.GetTypeByMetadataName($"{ns}.{typeName}")
                ?? throw new NullReferenceException($"{ns}.{typeName} not found.");
        }
    }

    private void GenerateInitialCode(IncrementalGeneratorPostInitializationContext context)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
        context.AddSource(
            hintName: $"Annotations.g.cs",
            source: AnnotationDefinitions.BuildAnnotationCode());
    }
}


