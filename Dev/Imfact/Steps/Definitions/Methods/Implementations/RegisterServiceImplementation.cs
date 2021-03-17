using Imfact.Interfaces;
using Imfact.Steps.Writing;
using Imfact.Steps.Writing.Coding;

namespace Imfact.Steps.Definitions.Methods
{
	internal record RegisterServiceImplementation(Property[] Delegations,
		Hook[] Hooks) : Implementation
	{
		public override void Render(ICodeBuilder builder, IResolverWriter writer)
		{
			builder.AppendLine("__resolverService = service;");

			foreach (var delegation in Delegations)
			{
				builder.AppendLine($"{delegation.Name}.RegisterService(service);");
			}

			foreach (var hook in Hooks)
			{
				builder.AppendLine($"{hook.FieldName}.RegisterService(service);");
			}
		}
	}
}
