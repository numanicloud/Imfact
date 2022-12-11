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

		var methods = FilteredMethods(syntax, context, ct);
		var baseTypes = FilterBaseTypes(symbol, ct);
		var resolutions = FilterResolutions(methods, ct);
		var delegations = FilterDelegations(symbol, ct);

		return new FilteredType(symbol,
			methods.AsRecordArray(),
			baseTypes.AsRecordArray(),
			resolutions.AsRecordArray(),
			delegations.AsRecordArray());
	}

	private static FilteredMethod[] FilteredMethods(
		TypeDeclarationSyntax syntax,
		GeneratorSyntaxContext context,
		CancellationToken ct)
	{
		return syntax.Members
			.OfType<MethodDeclarationSyntax>()
			.Select(TransformResolver)
			.FilterNull()
			.ToArray();
		
		FilteredMethod? TransformResolver(MethodDeclarationSyntax method)
		{
			ct.ThrowIfCancellationRequested();
			return context.SemanticModel.GetDeclaredSymbol(method, ct) is { } ms
				? new FilteredMethod(ms)
				: null;
		}
	}

	private FilteredDependency[] FilterBaseTypes(INamedTypeSymbol symbol, CancellationToken ct)
	{
		return TraverseBase(symbol)
			.Select(x => TransformDependency(x, ct))
			.FilterNull()
			.ToArray();

		IEnumerable<INamedTypeSymbol> TraverseBase(INamedTypeSymbol pivot)
		{
			if (pivot.BaseType is not { } baseType) yield break;

			yield return baseType;
			foreach (var child in TraverseBase(baseType))
			{
				yield return child;
			}
		}
	}

	private FilteredDependency[] FilterResolutions(FilteredMethod[] methods, CancellationToken ct)
	{
		return methods
			.Select(x => x.Symbol.ReturnType as INamedTypeSymbol)
			.FilterNull()
			.Select(x => TransformDependency(x, ct))
			.FilterNull()
			.ToArray();
	}

	private FilteredDependency[] FilterDelegations(
		INamedTypeSymbol owner,
		CancellationToken ct)
	{
		return owner.GetMembers()
			.OfType<IPropertySymbol>()
			.Select(TransformProperty)
			.FilterNull()
			.ToArray();

		FilteredDependency? TransformProperty(IPropertySymbol property)
		{
			ct.ThrowIfCancellationRequested();
			return property.Type is INamedTypeSymbol named
				? TransformDependency(named, ct)
				: null;
		}
	}

	private FilteredDependency? TransformDependency(
		INamedTypeSymbol typeToConstruct,
		CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();
		return typeToConstruct.IsReferenceType
			&& !typeToConstruct.IsRecord
			&& !typeToConstruct.IsAbstract
				? new FilteredDependency(typeToConstruct)
				: null;
	}

	public FilteredType? Match(
		(FilteredType? type, AnnotationContext annotations) tuple,
		CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();
		if (tuple.type is null) return null;
		if (!GeneralRule.Instance.IsFactoryCandidate(tuple.type.Symbol, tuple.annotations))
			return null;

		var baseTypes = tuple.type.BaseTypes
			.Where(x => GeneralRule.Instance.IsFactoryReference(x.Symbol, tuple.annotations))
			.ToArray();

		var resolutions = tuple.type.Resolutions
			.Where(x => GeneralRule.Instance.IsFactoryReference(x.Symbol, tuple.annotations))
			.ToArray();

		var delegations = tuple.type.Delegations
			.Where(x => GeneralRule.Instance.IsFactoryReference(x.Symbol, tuple.annotations))
			.ToArray();

		return tuple.type with
		{
			BaseTypes = baseTypes.AsRecordArray(),
			Resolutions = resolutions.AsRecordArray(),
			Delegations = delegations.AsRecordArray()
		};
	}
}