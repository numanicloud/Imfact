using System;
using System.Linq;
using Imfact.Entities;
using Imfact.Steps.Aspects;
using Imfact.Steps.Semanticses.Records;

namespace Imfact.Steps.Semanticses.Rules;

internal sealed class FactoryRule
{
	private readonly ResolverRule _resolverRule;

	public FactoryRule(ResolverRule resolverRule)
	{
		_resolverRule = resolverRule;
	}

	public Factory ExtractFactory(ClassAspect aspect)
	{
		var common = GetFactoryCommon(aspect.Type, aspect.Methods, ExtractImplementations(aspect));
		return new Factory(common,
			ExtractDelegations(aspect.Properties),
			ExtractInheritance(aspect.BaseClasses));
	}

	private static Implementation[] ExtractImplementations(ClassAspect aspect)
	{
		return aspect.Implements
			.Select(x => new Implementation(x.Type))
			.ToArray();
	}

	private FactoryCommon GetFactoryCommon(TypeAnalysis type, MethodAspect[] methods, Implementation[] implementations)
	{
		return new FactoryCommon(type,
			_resolverRule.ExtractResolver(methods),
			_resolverRule.ExtractMultiResolver(methods),
			implementations);
	}

	private Inheritance[] ExtractInheritance(ClassAspect[] baseClasses)
	{
		return baseClasses.Select(x =>
		{
			var parameters = x.KnownConstructor?.Parameters
				.Select(y => new Parameter(y.Type, y.Name))
				.ToArray();
			return new Inheritance(
				GetFactoryCommon(x.Type, x.Methods, ExtractImplementations(x)),
				parameters ?? new Parameter[0]);
		}).ToArray();
	}

	private Delegation[] ExtractDelegations(PropertyAspect[] properties)
	{
		return properties.Select(x =>
			{
				var hasRegisterService = x.MethodsInType
					.Any(y => y.Name == "RegisterService"
						&& y.Parameters.Length == 1
						&& y.Parameters[0].Type.FullBoundName == "Imfact.Annotations.ResolverService");
				var common = GetFactoryCommon(x.Type, x.MethodsInType, Array.Empty<Implementation>());
				return new Delegation(common, x.Name, hasRegisterService, x.IsAutoImplemented);
			})
			.ToArray();
	}
}