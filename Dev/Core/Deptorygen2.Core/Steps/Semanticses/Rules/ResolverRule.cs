using System.Linq;
using Deptorygen2.Annotations;
using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Steps.Aspects;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using Deptorygen2.Core.Utilities;
using NacHelpers.Extensions;
using Parameter = Deptorygen2.Core.Steps.Semanticses.Nodes.Parameter;

namespace Deptorygen2.Core.Steps.Semanticses.Rules
{
	internal sealed class ResolverRule
	{
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
				"__" + method.Name,
				parameters, resolutions, hooks);
		}

		private static Resolution ExtractResolution(TypeToCreate t)
		{
			return new(t.Node, t.ConstructorArguments, t.Node.DisposableType);
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
			var name = $"_{method.Name}_{typeNode.Name}";
			return new Hook(typeNode, name);
		}
	}
}
