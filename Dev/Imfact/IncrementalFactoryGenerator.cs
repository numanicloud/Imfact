using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Imfact.Annotations;
using Imfact.Main;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Imfact;

internal class PluginGlobal
{
	private static bool IsAttached = false;

	public static void Debug()
	{
		if (!Debugger.IsAttached && !IsAttached)
		{
			Debugger.Launch();
            Debugger.Break();

			IsAttached = true;
		}
	}
}

[Generator]
public class IncrementalFactoryGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
	{
        PluginGlobal.Debug();

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
		var sources = facade.Run(new[] { candidate });

        context.AddSource(
			hintName: $"{candidate.Symbol.GetFullNameSpace()}.{candidate.Symbol.Name}.g.cs",
			source: sources[0].Contents);
    }

    private FactoryCandidate? PostTransform(
        (FactoryIncremental? factory, AnnotationContext annotations) tuple,
        CancellationToken ct)
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

    private bool Predicate(SyntaxNode node, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return node is TypeDeclarationSyntax { AttributeLists.Count: > 0 };
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

internal record FactoryIncremental(INamedTypeSymbol Symbol, ResolverIncremental[] Methods);

internal record ResolverIncremental(IMethodSymbol Symbol, bool IsToGenerate);

internal record FactoryCandidate
    (INamedTypeSymbol Symbol,
    ResolverCandidate[] Methods,
    AnnotationContext? Context = null);

internal record ResolverCandidate(IMethodSymbol Symbol, bool IsToGenerate);

internal sealed class AnnotationContext
{
    public required INamedTypeSymbol FactoryAttribute { get; init; }
    public required INamedTypeSymbol ResolutionAttribute { get; init; }
    public required INamedTypeSymbol HookAttribute { get; init; }
    public required INamedTypeSymbol ExporterAttribute { get; init; }
    public required INamedTypeSymbol CacheAttribute { get; init; }
    public required INamedTypeSymbol CachePerResolutionAttribute { get; init; }
    public required INamedTypeSymbol TransientAttribute { get; init; }
}