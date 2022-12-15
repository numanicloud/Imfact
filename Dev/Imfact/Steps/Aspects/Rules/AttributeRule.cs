using Imfact.Annotations;
using Imfact.Entities;
using Imfact.Steps.Filter.Wrappers;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Aspects.Rules;

internal class AttributeRule
{
	private readonly TypeRule _typeRule;

	public AttributeRule(TypeRule typeRule)
	{
		_typeRule = typeRule;
	}

	public MethodAttributeAspect? ExtractAspect(AttributeData data,
		INamedTypeSymbol ownerReturn,
		string ownerName)
	{
		throw new NotImplementedException();
	}

	private (AnnotationKind, TypeToCreate)? Resolution(AttributeData data,
		INamedTypeSymbol ownerReturn)
	{
		if (data.ConstructorArguments.Length == 1
			&& data.ConstructorArguments[0].Kind == TypedConstantKind.Type
			&& data.ConstructorArguments[0].Value is INamedTypeSymbol t)
		{
			var kind = AnnotationKind.Resolution;
			var type = _typeRule.ExtractTypeToCreate(t, ownerReturn);
			return (kind, type);
		}

		return null;
	}

	private (AnnotationKind, TypeToCreate)? ResolutionT(AttributeData data,
		INamedTypeSymbol ownerReturn)
	{
		if (data.ConstructorArguments.Length == 0
			&& data.AttributeClass?.TypeArguments.Length == 1
			&& data.AttributeClass.TypeArguments[0] is INamedTypeSymbol t)
		{
			var type = _typeRule.ExtractTypeToCreate(t, ownerReturn);
			return (AnnotationKind.Resolution, type);
		}

		return null;
	}

	private (AnnotationKind, TypeToCreate)? Hook(AttributeData data,
		INamedTypeSymbol ownerReturn)
	{
		if (data.ConstructorArguments.Length == 1
			&& data.ConstructorArguments[0].Kind == TypedConstantKind.Type
			&& data.ConstructorArguments[0].Value is INamedTypeSymbol arg
			&& arg.ConstructedFrom.IsImplementing(typeof(IHook<>)))
		{
			var kind = AnnotationKind.Hook;
			var type = _typeRule.ExtractTypeToCreate(arg, ownerReturn);
			return (kind, type);
		}

		return null;
	}

	private static TypeToCreate PresetCache(Type hookType, INamedTypeSymbol ownerReturn)
	{
		var node = TypeAnalysis.FromRuntime(hookType,
			new[] {TypeAnalysis.FromSymbol(ownerReturn)});
		return new TypeToCreate(node, new TypeAnalysis[0]);
	}
}