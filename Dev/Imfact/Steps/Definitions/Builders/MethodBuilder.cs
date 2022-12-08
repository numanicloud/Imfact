using System;
using System.Linq;
using Imfact.Annotations;
using Imfact.Entities;
using Imfact.Steps.Definitions.Methods;
using Imfact.Steps.Definitions.Methods.Implementations;
using Imfact.Steps.Dependency;
using Imfact.Steps.Semanticses.Interfaces;
using Imfact.Steps.Semanticses.Records;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;
using Implementation = Imfact.Steps.Definitions.Methods.Implementation;

namespace Imfact.Steps.Definitions.Builders;

internal record Initialization(TypeAnalysis Type, string Name, string ParamName);

internal sealed class MethodBuilder
{
	private readonly InjectionResult _injection;
	private readonly MethodService _service;

	public MethodBuilder(MethodService service, InjectionResult injection, DisposeMethodBuilder disposeMethodBuilder, ConstructorBuilder constructorBuilder)
	{
		_service = service;
		_injection = injection;
		DisposeMethodBuilder = disposeMethodBuilder;
		ConstructorBuilder = constructorBuilder;
	}

	public DisposeMethodBuilder DisposeMethodBuilder { get; }

	public ConstructorBuilder ConstructorBuilder { get; }

	public static readonly TypeAnalysis ResolverServiceType = TypeAnalysis.FromRuntime(typeof(ResolverService));

	public MethodInfo? BuildRegisterServiceMethodInfo(Inheritance[] inheritances, Delegation[] delegations)
	{
		if (inheritances.Any())
		{
			return null;
		}

		var signature = new OrdinalSignature(Accessibility.Internal,
			TypeAnalysis.FromRuntime(typeof(void)),
			"RegisterService",
			new[] { new Parameter(ResolverServiceType, "service", false) },
			new string[0]);

		var p = delegations
			.Where(x => x.HasRegisterServiceMethod)
			.Select(x => new Property(x.PropertyName))
			.ToArray();

		var impl = new RegisterServiceImplementation(p, _service.ExtractHooks());
		return new MethodInfo(signature, impl);
	}

	public MethodInfo[] BuildResolverInfo(Resolver[] resolvers)
	{
		return resolvers
			.Select(x =>
			{
				return BuildMethodCommon(x, hooks1 =>
				{
					var exp = _injection.Creation[x].Root.Code;
					return new ExpressionImplementation(hooks1, x.ReturnType, exp);
				});
			}).ToArray();
	}

	public MethodInfo[] BuildEnumerableMethodInfo(MultiResolver[] multiResolvers)
	{
		return multiResolvers
			.Select(x =>
			{
				return BuildMethodCommon(x, hooks1 =>
				{
					var exp = _injection.MultiCreation[x].Roots.Select(y => y.Code).ToArray();
					return new MultiExpImplementation(hooks1, x.ElementType, exp);
				});
			}).ToArray();
	}

	private MethodInfo BuildMethodCommon(IResolverSemantics x,
		Func<Hook[], Implementation> makeImpl)
	{
		var ps = x.Parameters.Select(
				p => _service.BuildParameter(p.Type, p.ParameterName))
			.ToArray();
		var hooks = x.Hooks.Select(y => new Hook(y.HookType, y.FieldName))
			.ToArray();

		var signature = new OrdinalSignature(x.Accessibility,
			x.ReturnType, x.MethodName, ps, new string[]{ "partial" });
		var impl = makeImpl(hooks);

		return new MethodInfo(signature, impl);
	}
}