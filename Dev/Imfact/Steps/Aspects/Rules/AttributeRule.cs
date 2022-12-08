using Imfact.Annotations;
using Imfact.Entities;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Aspects.Rules;

internal class AttributeRule
{
	private readonly TypeRule _typeRule;
    private readonly AnnotationContext _annotations;

	public AttributeRule(TypeRule typeRule, AnnotationContext annotations)
	{
		_typeRule = typeRule;
        _annotations = annotations;
    }

	public MethodAttributeAspect? ExtractAspect(AttributeData data,
		INamedTypeSymbol ownerReturn,
		string ownerName)
	{
		if (data.AttributeClass is not { } attr)
		{
			return null;
		}

		AnnotationKind kind;
		TypeToCreate? type;

		if (Match(_annotations.ResolutionAttribute, attr))
		{
			if (Resolution(data, ownerReturn) is not { } tuple)
			{
				return null;
			}

			(kind, type) = tuple;
		}
		else if (Match(_annotations.ResolutionAttributeT, attr.OriginalDefinition))
		{
			if (ResolutionT(data, ownerReturn) is not { } tuple)
			{
				return null;
			}

			(kind, type) = tuple;
		}
		else if (Match(_annotations.HookAttribute, attr))
		{
			if (Hook(data, ownerReturn) is not {} tuple)
			{
				return null;
			}

			(kind, type) = tuple;
		}
		else if (Match(_annotations.CacheAttribute, attr))
		{
			kind = AnnotationKind.CacheHookPreset;
			type = PresetCache(typeof(Cache<>), ownerReturn);
		}
		else if (Match(_annotations.CachePerResolutionAttribute, attr))
		{
			kind = AnnotationKind.CachePrHookPreset;
			type = PresetCache(typeof(CachePerResolution<>), ownerReturn);
		}
		else
		{
			return null;
		}

		return new MethodAttributeAspect(kind, TypeAnalysis.FromSymbol(ownerReturn), ownerName, type);

		bool Match(INamedTypeSymbol actual, INamedTypeSymbol expected)
		{
			return SymbolEqualityComparer.Default.Equals(actual, expected);
		}
	}

	private (AnnotationKind, TypeToCreate)? Resolution(AttributeData data,
		INamedTypeSymbol ownerReturn)
	{
		using var profiler = TimeProfiler.Create("Attribute-Resolution");
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
		using var profiler = TimeProfiler.Create("Attribute-Hook");
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
		using var profiler = TimeProfiler.Create("Attribute-Preset");
		var node = TypeAnalysis.FromRuntime(hookType,
			new[] {TypeAnalysis.FromSymbol(ownerReturn)});
		return new TypeToCreate(node, new TypeAnalysis[0]);
	}
}