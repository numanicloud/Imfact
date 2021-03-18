using Imfact.Steps.Definitions.Interfaces;

namespace Imfact.Steps.Definitions.Methods
{
	internal record ConstructorImplementation
		(Initialization[] Initializations, Hook[] Hooks)
		: Implementation
	{
		public override void Render(IFluentCodeBuilder builder, IResolverWriter writer)
		{
			builder.EnterSequence(inner =>
			{
				inner.EnterChunk(chunk =>
				{
					foreach (var item in Initializations)
					{
						chunk.AppendLine($"{item.Name} = {item.ParamName};");
					}
				});

				inner.EnterChunk(chunk =>
				{
					foreach (var hook in Hooks)
					{
						var typeName = hook.TypeAnalysis.FullBoundName;
						chunk.AppendLine($"{hook.FieldName} = new {typeName}();");
					}
				});

				inner.EnterChunk(chunk =>
				{
					chunk.AppendLine("__resolverService = new ResolverService();");
					chunk.AppendLine("this.RegisterService(__resolverService);");
				});
			});
		}
	}
}
