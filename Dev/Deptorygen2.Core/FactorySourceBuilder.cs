using System;
using System.Linq;
using System.Text;

namespace Deptorygen2.Core
{
	public class FactorySourceBuilder
	{
		public string Build(FactoryClass factory)
		{
			return RenderFactory(factory);
		}

		public string RenderFactory(FactoryClass factory)
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

		public string RenderMethod(FactoryMethod method)
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

		private string RenderAnnotations(Span<HookAnnotation> annotations, string plainCode, string methodName)
		{
			if (annotations.IsEmpty)
			{
				return plainCode;
			}

			var inner = RenderAnnotations(annotations[1..], plainCode, methodName);
			var argument = $"() => {inner}".Indent(1);
			return $"_{methodName}_{annotations[0].HookClass.Name}.Hook(\n{argument})";
		}

		public string RenderParameter(FactoryMethodParameter parameter)
		{
			return $"{parameter.Type.Name} {parameter.Name}";
		}

		public string RenderField(DependencyField field)
		{
			return $"private readonly {field.Type.Name} {field.Name};";
		}

		public string RenderResolution(ResolutionConstructor resolution)
		{
			return $"new {resolution.ResolutionType.Name}({resolution.Arguments.Join(", ")})";
		}
	}

	class BlockStringBuilder : IDisposable
	{
		private readonly StringBuilder _target;
		private readonly string _indentPart;

		public BlockStringBuilder(string title, StringBuilder target, int indent)
		{
			_target = target;

			_indentPart = Enumerable.Range(0, indent).Aggregate("", (s, i) => s + "\t");
			_target.AppendLine(_indentPart + title);
			_target.AppendLine(_indentPart + "{");
		}

		public void Dispose()
		{
			_target.AppendLine(_indentPart + "}");
		}
	}
}
