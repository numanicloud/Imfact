using System;
using System.Collections.Generic;
using System.Text;
using Imfact.Steps.Writing;
using Imfact.Steps.Writing.Coding;

namespace Imfact.Steps.Definitions.Methods
{
	internal record ConstructorImplementation(Initialization[] Initializations,
		Hook[] Hooks) : Implementation
	{
		public override void Render(ICodeBuilder builder, ResolverWriter writer)
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
						var typeName = hook.FieldType.TypeName.FullBoundName;
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
