using System;
using System.Linq;
using System.Threading;
using Imfact.Annotations;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Imfact;

internal class IncrementalFactoryGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterPostInitializationOutput(GenerateInitialCode);

		IncrementalValueProvider<AnnotationContext> annotations =
			context.CompilationProvider
				.Select(ExtractAnnotations);

		IncrementalValuesProvider<FactoryCandidate> candidates =
			context.SyntaxProvider
				.CreateSyntaxProvider(Predicate, Transform)
				.Where(x => x != null)!;
	}

	private FactoryCandidate? Transform(GeneratorSyntaxContext context, CancellationToken ct)
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
			? new FactoryCandidate(symbol, methods)
			: null;

		ResolverCandidate? GetResolver(MethodDeclarationSyntax method)
		{
			return context.SemanticModel.GetDeclaredSymbol(method) is { } ms
				? new ResolverCandidate(ms,
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
		return node is TypeDeclarationSyntax { AttributeLists.Count: > 0 } syntax
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

public record FactoryCandidate(INamedTypeSymbol Symbol, ResolverCandidate[] Methods);

public record ResolverCandidate(IMethodSymbol Symbol, bool IsToGenerate);

internal sealed class AnnotationContext
{
	public required INamedTypeSymbol FactoryAttribute { private get; init; }
	public required INamedTypeSymbol ResolutionAttribute { private get; init; }
	public required INamedTypeSymbol HookAttribute { private get; init; }
	public required INamedTypeSymbol ExporterAttribute { private get; init; }
	public required INamedTypeSymbol CacheAttribute { private get; init; }
	public required INamedTypeSymbol CachePerResolutionAttribute { private get; init; }
	public required INamedTypeSymbol TransientAttribute { private get; init; }
}