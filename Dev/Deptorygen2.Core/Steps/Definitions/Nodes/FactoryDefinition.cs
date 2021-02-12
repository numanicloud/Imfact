using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Definitions
{
	public record FactoryDefinition(string Name,
		ResolverDefinition[] Methods,
		CollectionResolverDefinition[] CollectionResolvers,
		DelegationDefinition[] Delegations,
		DependencyDefinition[] Fields,
		FactoryConstructorDefinition Constructor)
	{
		public IEnumerable<ResolverParameterDefinition> GetConstructorFieldParameters()
		{
			foreach (var field in Fields)
			{
				var name = field.FieldName.TrimStart("_".ToCharArray());
				yield return new ResolverParameterDefinition(field.FieldType, name);
			}
		}

		public IEnumerable<ResolverParameterDefinition> GetConstructorPropertyParameters()
		{
			foreach (var delegation in Delegations)
			{
				var name = delegation.PropertyName.ToLowerCamelCase();
				yield return new ResolverParameterDefinition(delegation.PropertyType, name);
			}
		}

		internal static FactoryDefinition Build(FactorySemantics semantics, ChildrenBuilder childrenBuilder)
		{
			return childrenBuilder(semantics.Type.Name);
		}

		internal delegate FactoryDefinition ChildrenBuilder(string className);
	}
}