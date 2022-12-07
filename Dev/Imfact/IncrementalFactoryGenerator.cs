using System;
using System.Threading;
using Imfact.Annotations;
using Microsoft.CodeAnalysis;

namespace Imfact;

internal class IncrementalFactoryGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterPostInitializationOutput(GenerateInitialCode);

		IncrementalValueProvider<AnnotationContext> annotations =
			context.CompilationProvider
				.Select(ExtractAnnotations);

		IncrementalValuesProvider<CandidateClass> candidates =
			context.SyntaxProvider
				.CreateSyntaxProvider(Predicate_, Transform);
	}

	private CandidateClass Transform(GeneratorSyntaxContext context, CancellationToken arg2)
	{
		// CandidateClass は partial なクラス。
		// partial なメソッドを表す CandidateMethod をCandidateClass に持たせる形で解析すると、
		// この後のステップで Syntax を一切使わずに済んで助かるかも。
		throw new NotImplementedException();
	}

	private bool Predicate_(SyntaxNode arg1, CancellationToken arg2)
	{
		throw new NotImplementedException();
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

public record FactoryCandidate(
	INamedTypeSymbol Symbol,
	ResolverCandidate[] Resolvers);

public record ResolverCandidate(IMethodSymbol Symbol);

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