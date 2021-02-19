using System.Linq;
using Deptorygen2.Annotations;
using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Aspects;
using Deptorygen2.Core.Steps.Aspects.Nodes;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using Deptorygen2.Core.Steps.Semanticses.Rules;
using Deptorygen2.Core.Utilities;
using NacHelpers.Extensions;
using Parameter = Deptorygen2.Core.Steps.Semanticses.Nodes.Parameter;

namespace Deptorygen2.Core.Steps.Semanticses
{
	internal sealed class ResolverRule
	{
		private readonly IAnalysisContext _context;
		private readonly HookRule _hookRule = new ();

		public ResolverRule(IAnalysisContext context)
		{
			_context = context;
		}

		public MultiResolver[] ExtractMultiResolver(MethodAspect[] methods)
		{
			return methods.Where(x => x.Kind == ResolverKind.Multi)
				.Select(x => new MultiResolver(GetMethodCommon(x)))
				.ToArray();
		}

		public Resolver[] ExtractResolver(MethodAspect[] methods)
		{
			return methods.Where(x => x.Kind == ResolverKind.Single)
				.Select(x => new Resolver(GetMethodCommon(x), ExtractResolution(x.ReturnType.Type)))
				.ToArray();
		}

		private ResolverCommon GetMethodCommon(MethodAspect method)
		{
			var ctx = new Parameter(TypeNode.FromRuntime(typeof(ResolutionContext)), "context");

			var parameters = method.Parameters.Select(x => new Parameter(x.Type, x.Name))
				.Append(ctx)
				.ToArray();

			var resolutions = method.Attributes.Where(x => x.Kind == AnnotationKind.Resolution)
				.Select(x => ExtractResolution(x.TypeToCreate))
				.ToArray();

			var hooks = method.Attributes.Select(x => ExtractHook(x, method))
				.FilterNull()
				.ToArray();

			return new ResolverCommon(method.Accessibility,
				method.ReturnType.Type.Node,
				method.Name, parameters, resolutions, hooks);
		}

		private static Resolution ExtractResolution(TypeToCreate t)
		{
			return new Resolution(t.Node, t.ConstructorArguments,
				t.Node.DisposableType == DisposableType.Disposable);
		}

		private Hook? ExtractHook(MethodAttributeAspect aspect, MethodAspect method)
		{
			if (aspect.Kind != AnnotationKind.Hook &&
			    aspect.Kind != AnnotationKind.CacheHookPreset &&
			    aspect.Kind != AnnotationKind.CachePrHookPreset)
			{
				return null;
			}

			var typeNode = aspect.TypeToCreate.Node;

			if (!typeNode.TypeArguments.Any())
			{
				typeNode = typeNode with
				{
					TypeArguments = new[] { method.ReturnType.Type.Node }
				};
			}

			var name = $"_{method.Name}_{typeNode.Name}";
			return new Hook(typeNode, name);
		}


		public MultiResolver[] ExtractMultiResolver(Method[] methods)
		{
			return methods.Where(x => x.IsCollectionResolver())
				.Select(m => new MultiResolver(GetMethodCommon(m)))
				.ToArray();
		}

		public Resolver[] ExtractResolver(Method[] getMethods)
		{
			return getMethods.Where(x => x.IsSingleResolver())
				.Select(m =>
				{
					var ret = m.GetReturnType(_context) is { } t
						? Resolution.Build(t)
						: null;
					var abstraction = GetMethodCommon(m);

					return new Resolver(abstraction, ret);
				}).ToArray();
		}

		private ResolverCommon GetMethodCommon(Method method)
		{
			var parameters = method.GetParameters();
			var attributes = method.GetAttributes();
			var ctx = new Parameter(TypeNode.FromRuntime(typeof(ResolutionContext)), "context");

			return new ResolverCommon(
				method.Symbol.DeclaredAccessibility,
				TypeNode.FromSymbol(method.Symbol.ReturnType),
				"__" + method.Symbol.Name,
				AggregateParameter(parameters).Append(ctx).ToArray(),
				AggregateResolution(attributes),
				AggregateHooks(attributes, method));
		}

		private Hook[] AggregateHooks(Attribute[] attributes, Method method)
		{
			return attributes.Select(x => _hookRule.Extract(x, method))
				.FilterNull()
				.ToArray();
		}

		private Parameter[] AggregateParameter(Aspects.Nodes.Parameter[] parameters)
		{
			return parameters.Select(x => Parameter.Build(x, _context))
				.FilterNull()
				.ToArray();
		}

		private Resolution[] AggregateResolution(Attribute[] attributes)
		{
			return attributes.Select(x => Resolution.Build(x, _context))
				.FilterNull()
				.ToArray();
		}
	}
}
