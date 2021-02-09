using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core
{
	public record SourceCodeDefinition(string[] Usings, string Namespace,
		FactoryDefinition Factory);

	public record FactoryDefinition(string Name,
		ResolverDefinition[] Methods,
		CollectionResolverDefinition[] CollectionResolvers,
		DelegationDefinition[] Delegations,
		DependencyDefinition[] Fields);

	public record DependencyDefinition(TypeName FieldType, string FieldName);

	public record ResolverDefinition(Accessibility AccessLevel,
		string Name,
		TypeName ReturnType,
		ResolutionDefinition Resolution,
		ResolverParameterDefinition[] Parameters,
		HookDefinition[] Hooks);

	public record CollectionResolverDefinition(Accessibility AccessLevel,
		string Name,
		TypeName ReturnType,
		ResolutionDefinition[] Resolutions,
		ResolverParameterDefinition[] Parameters,
		HookDefinition[] Hooks);

	public record DelegationDefinition(TypeName PropertyType,
		string PropertyName);

	public record TypeInfo(string Namespace, string Name);

	public record ResolverParameterDefinition(TypeName Type, string Name);

	public record HookDefinition(TypeName HookClass);

	public record ResolutionDefinition(TypeName ResolutionType, string[] ConstructorArguments);
}
