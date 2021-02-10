using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Definitions
{
	public record FactoryConstructorDefinition(
		Dictionary<DependencyDefinition, ResolverParameterDefinition> FieldParameter,
		Dictionary<DelegationDefinition, ResolverParameterDefinition> PropertyParameter)
	{
		public static FactoryConstructorDefinition Build(
			DependencyDefinition[] dependencies, DelegationDefinition[] delegations)
		{
			var f = dependencies.Select(x =>
			{
				var name = x.FieldName.TrimStart("_".ToCharArray());
				return (x, new ResolverParameterDefinition(x.FieldType, name));
			}).ToDictionary(t => t.x, t => t.Item2);

			var p = delegations.Select(x =>
			{
				var name = x.PropertyName.ToLowerCamelCase();
				return (x, new ResolverParameterDefinition(x.PropertyType, name));
			}).ToDictionary(t => t.x, t => t.Item2);

			return new FactoryConstructorDefinition(f, p);
		}
	}
}
