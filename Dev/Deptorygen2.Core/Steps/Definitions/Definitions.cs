using Deptorygen2.Core.Steps.Instantiation;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Definitions
{
	public record DependencyDefinition(TypeName FieldType, string FieldName);

	public record DelegationDefinition(TypeName PropertyType,
		string PropertyName,
		ResolverDefinition[] Resolvers,
		CollectionResolverDefinition[] CollectionResolvers);

	public record ResolverParameterDefinition(TypeName Type, string Name);

	public record HookDefinition(TypeName HookClass);

	public record ResolutionDefinition(TypeName TypeToResolve);
}
