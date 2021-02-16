using System.Linq;
using Deptorygen2.Annotations;
using Deptorygen2.Core.Steps.Creation;
using Deptorygen2.Core.Steps.Creation.Abstraction;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using Deptorygen2.Core.Steps.Writing;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Definitions.Methods
{
	internal record InitializeImplementation(Assignment[] Assignments) : Implementation
	{
		public override void Render(ICodeBuilder builder, ICreationAggregator creation,
			ResolverWriter writer)
		{
			foreach (var assignment in Assignments)
			{
				builder.AppendLine($"{assignment.Dest} = {assignment.Src};");
			}
		}
	}

	internal record DisposeImplementation(Field[] Fields, Property[] Properties, bool IsAsync) : Implementation
	{
		public override void Render(ICodeBuilder builder, ICreationAggregator creation,
			ResolverWriter writer)
		{
			var op = IsAsync ? "await " : "";
			var method = IsAsync ? "DisposeAsync" : "Dispose";

			foreach (var field in Fields)
			{
				builder.AppendLine(op + $"{field.Name}.{method}();");
			}

			foreach (var property in Properties)
			{
				builder.AppendLine(op + $"{property.Name}.{method}();");
			}
		}
	}

	internal record EntryImplementation(string Name, Parameter[] Parameters) : Implementation
	{
		public override void Render(ICodeBuilder builder, ICreationAggregator creation,
			ResolverWriter writer)
		{
			var argList = Parameters.Select(x => x.Name)
				.Append("context")
				.Join(", ");

			builder.AppendLine("var context = new ResolutionContext();");
			builder.AppendLine($"return __{Name}({argList});");
		}
	}

	internal record ResolveImplementation(Hook[] Hooks, Type Resolution,
		Type ReturnType, Parameter[] Parameters) : Implementation
	{
		public override void Render(ICodeBuilder builder, ICreationAggregator creation,
			ResolverWriter writer)
		{
			var given = Parameters
				.Select(x => new GivenParameter(x.Type.TypeName, x.Name))
				.ToArray();
			var request = new CreationRequest(Resolution.TypeName, given, true);
			var expression = creation.GetInjection(request) ?? "__NoResolutionFound__";

			writer.Render(ReturnType.Text, Hooks, expression, builder);
		}
	}

	internal record MultiResolveImplementation(Hook[] Hooks, Type ElementType,
		Type[] Resolutions, Parameter[] Parameters) : Implementation
	{
		public override void Render(ICodeBuilder builder, ICreationAggregator creation, ResolverWriter writer)
		{
			var given = Parameters
				.Select(x => new GivenParameter(x.Type.TypeName, x.Name))
				.ToArray();
			var typeToResolve = Resolutions.Select(x => x.TypeName).ToArray();
			var request = new MultipleCreationRequest(typeToResolve, given, true);

			var expBuilder = CodeHelper.GetBuilder();
			expBuilder.AppendLine($"new {ElementType.Text}[]");
			expBuilder.EnterBlock(inner =>
			{
				inner.AppendLine(creation.GetInjections(request).Join(",\n"));
			});

			var returnType = $"IEnumerable<{ElementType.Text}>";
			writer.Render(returnType, Hooks, expBuilder.GetText(), builder);
		}
	}
}
