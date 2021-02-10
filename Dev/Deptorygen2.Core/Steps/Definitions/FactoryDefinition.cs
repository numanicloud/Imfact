using Deptorygen2.Core.Steps.Semanticses;

namespace Deptorygen2.Core.Steps.Definitions
{
	public record FactoryDefinition(string Name,
		ResolverDefinition[] Methods,
		CollectionResolverDefinition[] CollectionResolvers,
		DelegationDefinition[] Delegations,
		DependencyDefinition[] Fields)
	{
		internal static FactoryDefinition Build(FactorySemantics semantics, ChildrenBuilder childrenBuilder)
		{
			return childrenBuilder(semantics.ItselfSymbol.Name);
		}

		internal delegate FactoryDefinition ChildrenBuilder(string className);
	}
}