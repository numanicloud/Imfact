using System;
using System.Linq;
using System.Text;
using Deptorygen2.Core.Steps.Definitions;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Writing
{
	public class SourceCodeBuilder
	{
		internal SourceFile Build(SourceCodeDefinition sourceCode)
		{
			var fileName = sourceCode.Factory.Name + ".g.cs";
			var contents = RenderFactory(sourceCode.Factory);
			return new SourceFile(fileName, contents);
		}

		private void AppendBlock(StringBuilder builder, string header, Action<StringBuilder> build)
		{
			builder.AppendLine(header);
			builder.AppendLine("{");

			var innerBuilder = new StringBuilder();
			build(innerBuilder);

			builder.AppendLine(innerBuilder.ToString().Indent(1).TrimEnd());
			builder.AppendLine("}");
		}

		public string RenderFactory(FactoryDefinition factory)
		{
			var builder = new StringBuilder();

			AppendBlock(builder, $"internal partial class {factory.Name}", innerBuilder =>
			{
				foreach (var factoryField in factory.Fields)
				{
					innerBuilder.AppendLine(RenderField(factoryField));
				}

				if (factory.Fields.Any())
				{
					innerBuilder.AppendLine();
					RenderConstructor(factory, innerBuilder);
					innerBuilder.AppendLine();
				}

				foreach (var factoryMethod in factory.Methods)
				{
					RenderMethod(factoryMethod, innerBuilder);
				}
			});

			return builder.ToString();
		}

		private void RenderConstructor(FactoryDefinition factory, StringBuilder builder)
		{
			var constructor = factory.Constructor;
			var argList = constructor.FieldParameter.Values
				.Concat(constructor.PropertyParameter.Values)
				.Select(RenderParameter)
				.Join(", ");

			AppendBlock(builder, $"public {factory.Name}({argList})", innerBuilder =>
			{
				foreach (var item in constructor.FieldParameter)
				{
					innerBuilder.AppendLine($"{item.Key.FieldName} = {item.Value.Name}");
				}

				foreach (var item in constructor.PropertyParameter)
				{
					innerBuilder.AppendLine($"{item.Key.PropertyName} = {item.Value.Name}");
				}
			});
		}

		public void RenderMethod(ResolverDefinition method, StringBuilder builder)
		{
			var parameterList = method.Parameters.Select(RenderParameter).Join(", ");
			var signature = string.Format("{0} partial {1} {2}({3})",
				method.AccessLevel.ToString().ToLower(),
				method.ReturnType.Name,
				method.Name,
				parameterList);
			
			AppendBlock(builder, signature, innerBuilder =>
			{
				var code = RenderAnnotations(
					method.Hooks.AsSpan(),
					RenderResolution(method.Resolution),
					method.Name);

				innerBuilder.AppendLine($"return {code};");
			});
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

		// TODO: コンストラクタではなく、コンストラクタの引数に与えるような値を直接返すパターンも欲しい
		public string RenderResolution(ResolutionDefinition resolution)
		{
			return resolution.ResolutionCode;
		}
	}
}
