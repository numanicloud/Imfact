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
		public static FactoryDefinition Build(FactorySyntax syntax, ChildrenBuilder childrenBuilder)
		{
			return childrenBuilder(syntax.ItselfSymbol.Name);
		}

		public delegate FactoryDefinition ChildrenBuilder(string className);
	}
}