using Imfact.Steps.Filter.Rules;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Imfact.Steps.Filter;

internal sealed class FilterStep
{
	public required ClassRule ClassRule { private get; init; }

    public bool IsFactory(SyntaxNode node, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return GeneralRule.Instance.IsFactoryClassDeclaration(node);
    }

    public FilteredType? Transform(GeneratorSyntaxContext context, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var syntax = (context.Node as TypeDeclarationSyntax)!;
		return ClassRule.TransformClass(syntax, context, ct);
	}

	public FilteredType? Match(
        (FilteredType? type, AnnotationContext annotations) tuple,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        if (tuple.type is null) return null;

		return ClassRule.Match(tuple.type, tuple.annotations, ct);
	}
}