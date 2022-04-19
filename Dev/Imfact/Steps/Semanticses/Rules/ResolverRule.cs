using System.Linq;
using Imfact.Entities;
using Imfact.Steps.Aspects;
using Imfact.Steps.Semanticses.Records;
using Imfact.Utilities;

namespace Imfact.Steps.Semanticses.Rules
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

		public Exporter[] ExtractExporters(ExporterAspect[] methods)
		{
			using var profiler = TimeProfiler.Create("Extract-Exporter-Semantics");
			return methods.Select(x => new Exporter(x.Name, new []
				{
					new Parameter(x.Parameters[0].Type, x.Parameters[0].Name),
					new Parameter(x.Parameters[1].Type, x.Parameters[1].Name)
				}))
				.ToArray();
		}

		private ResolverCommon GetMethodCommon(MethodAspect method)
		{
			using var profiler = TimeProfiler.Create("Extract-MethodCommon-Semantics");
			var parameters = method.Parameters.Select(x => new Parameter(x.Type, x.Name))
				.ToArray();

			var resolutions = method.Attributes.Where(x => x.Kind == AnnotationKind.Resolution)
				.Select(x => ExtractResolution(x.TypeToCreate))
				.ToArray();

			var hooks = method.Attributes.Select(x => ExtractHook(x, method))
				.FilterNull()
				.ToArray();

			return new ResolverCommon(method.Accessibility,
				method.ReturnType.Type.Info,
				method.Name,
				parameters, resolutions, hooks);
		}

		private static Resolution ExtractResolution(TypeToCreate t)
		{
			return new(t.Info, t.ConstructorArguments, t.Info.DisposableType);
		}

		private Hook? ExtractHook(MethodAttributeAspect aspect, MethodAspect method)
		{
			using var profiler = TimeProfiler.Create("Extract-Hook-Semantics");
			if (aspect.Kind != AnnotationKind.Hook &&
			    aspect.Kind != AnnotationKind.CacheHookPreset &&
			    aspect.Kind != AnnotationKind.CachePrHookPreset)
			{
				return null;
			}

			var typeNode = aspect.TypeToCreate.Info;
			var name = $"_{method.Name}_{typeNode.Name}";
			return new Hook(typeNode, name);
		}
	}
}
