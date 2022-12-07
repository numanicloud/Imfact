using System;
using Imfact.Annotations;
using Imfact.Entities;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Aspects.Rules;

internal class AttributeRule
{
	private readonly TypeRule _typeRule;
	private static readonly AttributeName _resAt = new(nameof(ResolutionAttribute));
	private static readonly AttributeName _hokAt = new(nameof(HookAttribute));
	private static readonly AttributeName _cacAt = new(nameof(CacheAttribute));
	private static readonly AttributeName _cprAt = new(nameof(CachePerResolutionAttribute));

	public AttributeRule(TypeRule typeRule)
	{
		_typeRule = typeRule;
	}

	public MethodAttributeAspect? ExtractAspect(AttributeData data,
		INamedTypeSymbol ownerReturn,
		string ownerName)
	{
		if (data.AttributeClass?.Name is not { } name)
		{
			return null;
		}

		AnnotationKind kind;
		TypeToCreate? type;

		if (_resAt.MatchWithAnyName(name))
		{
			if (Resolution(data, ownerReturn) is not { } tuple)
			{
				return null;
			}

			(kind, type) = tuple;
		}
		else if (_hokAt.MatchWithAnyName(name))
		{
			if (Hook(data, ownerReturn) is not {} tuple)
			{
				return null;
			}

			(kind, type) = tuple;
		}
		else if (_cacAt.MatchWithAnyName(name))
		{
			kind = AnnotationKind.CacheHookPreset;
			type = PresetCache(typeof(Cache<>), ownerReturn);
		}
		else if (_cprAt.MatchWithAnyName(name))
		{
			kind = AnnotationKind.CachePrHookPreset;
			type = PresetCache(typeof(CachePerResolution<>), ownerReturn);
		}
		else
		{
			return null;
		}

		return new MethodAttributeAspect(kind, TypeAnalysis.FromSymbol(ownerReturn), ownerName, type);
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