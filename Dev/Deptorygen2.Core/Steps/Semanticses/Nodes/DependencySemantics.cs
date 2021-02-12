using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Semanticses
{
	internal record DependencySemantics(TypeName TypeName, string FieldName) : INamespaceClaimer
	{
		public static DependencySemantics[] FromFactory(FactorySemantics semantics)
		{
			var consumers = semantics.Resolvers.Cast<IServiceConsumer>()
				.Concat(semantics.CollectionResolvers)
				.SelectMany(x => x.GetRequiredServiceTypes())
				.Distinct();

			var providers = semantics.Resolvers.Cast<IServiceProvider>()
				.Concat(semantics.CollectionResolvers)
				.Concat(semantics.Delegations)
				.SelectMany(x => x.GetCapableServiceTypes())
				.Distinct();

			return consumers.Except(providers)
				.Select(t => new DependencySemantics(t, "_" + t.Name.ToLowerCamelCase()))
				.ToArray();
		}

		public IEnumerable<string> GetRequiredNamespaces()
		{
			yield return TypeName.FullNamespace;
		}
	}
}
