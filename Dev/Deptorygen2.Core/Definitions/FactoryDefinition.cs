using System;
using Deptorygen2.Core.Syntaxes;

namespace Deptorygen2.Core
{
	internal record FactoryDefinition(string Name,
		ResolverDefinition[] Methods,
		CollectionResolverDefinition[] CollectionResolvers,
		DelegationDefinition[] Delegations,
		DependencyDefinition[] Fields)
	{
		public static FactoryDefinition Build(FactorySemantics semantics, ChildrenBuilder childrenBuilder)
		{
			return childrenBuilder(semantics.ItselfSymbol.Name);
		}

		public delegate FactoryDefinition ChildrenBuilder(string className);
	}
}