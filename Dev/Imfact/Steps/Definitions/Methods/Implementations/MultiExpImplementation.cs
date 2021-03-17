using Imfact.Entities;
using Imfact.Steps.Writing;
using Imfact.Steps.Writing.Coding;
using Imfact.Utilities;

namespace Imfact.Steps.Definitions.Methods
{
	internal record MultiExpImplementation(Hook[] Hooks, TypeAnalysis ElementType, string[] Expressions)
		: Implementation
	{
		public override void Render(ICodeBuilder builder, ResolverWriter writer)
		{
			var expBuilder = CodeHelper.GetBuilder();
			expBuilder.AppendLine($"new {ElementType.GetCode()}[]");
			expBuilder.EnterBlock(inner =>
			{
				inner.AppendLine(Expressions.Join(",\n"));
			});

			var returnType = $"IEnumerable<{ElementType.GetCode()}>";
			writer.Render(returnType, Hooks, expBuilder.GetText(), builder);
		}
	}
}
