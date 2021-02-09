using System;
using System.Linq;
using System.Text;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core
{
	public class FactorySourceBuilder
	{
		public string Build(FactoryDefinition factory)
		{
			return RenderFactory(factory);
		}

		public string RenderFactory(FactoryDefinition factory)
		{
			var builder = new StringBuilder();

			using (new BlockStringBuilder($"internal class {factory.Name}", builder, 0))
			{
				foreach (var factoryField in factory.Fields)
				{
					builder.AppendLine(RenderField(factoryField).Indent(1));
				}

				foreach (var factoryMethod in factory.Methods)
				{
					builder.AppendLine(RenderMethod(factoryMethod).Indent(1));
				}
			}

			return builder.ToString();
		}

		public string RenderMethod(ResolverDefinition method)
		{
			var parameterList = method.Parameters.Select(RenderParameter).Join(", ");
			var signature = string.Format("{0} partial {1} {2}({3})",
				method.AccessLevel.ToString().ToLower(),
				method.ReturnType.Name,
				method.Name,
				parameterList);

			var builder = new StringBuilder();
			using (new BlockStringBuilder(signature, builder, 0))
			{
				var code = RenderAnnotations(
					method.Hooks.AsSpan(),
					RenderResolution(method.Resolution),
					method.Name);

				builder.AppendLine($"return {code};".Indent(1));
			}

			return builder.ToString();
		}

		private string RenderAnnotations(Span<HookDefinition> annotations, string plainCode, string methodName)
		{
			if (annotations.IsEmpty)
			{
				return plainCode;
			}

			var inner = RenderAnnotations(annotations[1..], plainCode, methodName);
			var argument = $"() => {inner}".Indent(1);
			return $"_{methodName}_{annotations[0].HookClass.Name}.Hook(\n{argument})";
		}

		public string RenderParameter(ResolverParameterDefinition parameterDefinition)
		{
			return $"{parameterDefinition.Type.Name} {parameterDefinition.Name}";
		}

		public string RenderField(DependencyDefinition definition)
		{
			return $"private readonly {definition.FieldType.Name} {definition.FieldName};";
		}

		public string RenderResolution(ResolutionDefinition resolution)
		{
			return $"new {resolution.ResolutionType.Name}({resolution.ConstructorArguments.Join(", ")})";
		}
	}
}
