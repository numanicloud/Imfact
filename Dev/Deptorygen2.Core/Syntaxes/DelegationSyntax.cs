using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Syntaxes
{
	internal record DelegationSyntax(string PropertyName,
		TypeName TypeName,
		ResolverSyntax[] Resolvers,
		CollectionResolverSyntax[] CollectionResolvers) : IServiceProvider, INamespaceClaimer
	{
		public IEnumerable<TypeName> GetCapableServiceTypes()
		{
			return Resolvers.Cast<IServiceProvider>()
				.Concat(CollectionResolvers)
				.SelectMany(x => x.GetCapableServiceTypes());
		}

		public IEnumerable<string> GetRequiredNamespaces()
		{
			yield return TypeName.FullNamespace;
		}
	}
}
