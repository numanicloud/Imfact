using Imfact.Steps.Definitions.Interfaces;

namespace Imfact.Steps.Definitions.Methods.Implementations;

internal record RegisterServiceImplementation(Property[] Delegations,
	Hook[] Hooks) : Implementation
{
	public override void Render(IFluentCodeBuilder builder, IResolverWriter writer)
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