using Imfact.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Imfact.Steps.Filter;

internal sealed class FilterStep
{
	private static FilterStep? _instance;
	public static FilterStep Instance => _instance ??= new FilterStep();

	private FilterStep()
	{
	}

	public bool IsFactory(SyntaxNode node, CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();
		return GeneralRule.Instance.IsFactoryClassDeclaration(node);
	}

	public FilteredType? Transform(GeneratorSyntaxContext context, CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();
		var syntax = (context.Node as TypeDeclarationSyntax)!;
		if (syntax.Modifiers.IndexOf(SyntaxKind.PartialKeyword) == -1) return null;

		var symbol = context.SemanticModel.GetDeclaredSymbol(syntax, ct);
		if (symbol is null) return null;

		var methods = syntax.Members
			.OfType<MethodDeclarationSyntax>()
			.Select(TransformResolver)
			.FilterNull()
			.ToArray();

		return new FilteredType(symbol, new RecordArray<FilteredMethod>(methods));

		FilteredMethod? TransformResolver(MethodDeclarationSyntax method)
		{
			ct.ThrowIfCancellationRequested();
			return context.SemanticModel.GetDeclaredSymbol(method) is {} ms
				? new FilteredMethod(ms)
				: null;
		}
	}

	public FilteredType? Match(
		(FilteredType? type, AnnotationContext annotations) tuple,
		CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();
		return tuple.type is null
			? null
			: GeneralRule.Instance.IsFactoryCandidate(tuple.type.Symbol, tuple.annotations)
				? tuple.type
				: null;
	}
}