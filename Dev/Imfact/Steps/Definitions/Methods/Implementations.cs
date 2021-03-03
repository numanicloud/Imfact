using System.Linq;
using Imfact.Steps.Writing;
using Imfact.Steps.Writing.Coding;
using Imfact.Utilities;

namespace Imfact.Steps.Definitions.Methods
{
	internal record InitializeImplementation(Assignment[] Assignments) : Implementation
	{
		public override void Render(ICodeBuilder builder, ResolverWriter writer)
		{
			foreach (var assignment in Assignments)
			{
				builder.AppendLine($"{assignment.Dest} = {assignment.Src};");
			}
		}
	}

	internal record DisposeImplementation(string[] MemberNames, bool IsAsync) : Implementation
	{
		public override void Render(ICodeBuilder builder, ResolverWriter writer)
		{
			var op = IsAsync ? "await " : "";
			var method = IsAsync ? "DisposeAsync" : "Dispose";

			foreach (var name in MemberNames)
			{
				builder.AppendLine(op + $"{name}.{method}();");
			}
		}
	}

	internal record EntryImplementation(string Name, Parameter[] Parameters) : Implementation
	{
		public override void Render(ICodeBuilder builder, ResolverWriter writer)
		{
			var argList = Parameters.Select(x => x.Name)
				.Append("context")
				.Join(", ");

			builder.AppendLine("var context = new ResolutionContext();");
			builder.AppendLine($"return {Name}({argList});");
		}
	}

	internal record ExpressionImplementation(Hook[] Hooks, Type ReturnType, string Expression) : Implementation
	{
		public override void Render(ICodeBuilder builder, ResolverWriter writer)
		{
			writer.Render(ReturnType.Text, Hooks, Expression, builder);
		}
	}

	internal record MultiExpImplementation(Hook[] Hooks, Type ElementType, string[] Expressions)
		: Implementation
	{
		public override void Render(ICodeBuilder builder, ResolverWriter writer)
		{
			var expBuilder = CodeHelper.GetBuilder();
			expBuilder.AppendLine($"new {ElementType.Text}[]");
			expBuilder.EnterBlock(inner =>
			{
				inner.AppendLine(Expressions.Join(",\n"));
			});

			var returnType = $"IEnumerable<{ElementType.Text}>";
			writer.Render(returnType, Hooks, expBuilder.GetText(), builder);
		}
	}
}
