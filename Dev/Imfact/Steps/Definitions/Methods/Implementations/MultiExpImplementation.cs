using Imfact.Entities;
using Imfact.Steps.Definitions.Interfaces;
using Imfact.Utilities;
using Imfact.Utilities.Coding;

namespace Imfact.Steps.Definitions.Methods.Implementations;

internal record MultiExpImplementation(Hook[] Hooks, TypeAnalysis ElementType, string[] Expressions)
	: Implementation
{
	public override void Render(IFluentCodeBuilder builder, IResolverWriter writer)
	{
		var exp = builder.OnPlainBuilder(plain =>
		{
			plain.AppendLine($"new {ElementType.GetCode()}[]");
			plain.EnterBlock(block =>
			{
				block.AppendLine(Expressions.Join(",\n"));
			});
		});

		var returnType = $"IEnumerable<{ElementType.GetCode()}>";
		writer.Render(returnType, Hooks, exp, builder);
	}
}