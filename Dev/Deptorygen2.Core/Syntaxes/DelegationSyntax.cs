using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Structure;
using Deptorygen2.Core.Syntaxes.Parser;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Syntaxes
{
	class DelegationSyntax : IServiceProvider
	{
		public string PropertyName { get; }
		public TypeName TypeName { get; }
		public ResolverSyntax[] Resolvers { get; }
		public CollectionResolverSyntax[] CollectionResolvers { get; }

		public DelegationSyntax(string propertyName, TypeName typeName, ResolverSyntax[] resolvers, CollectionResolverSyntax[] collectionResolvers)
		{
			PropertyName = propertyName;
			TypeName = typeName;
			Resolvers = resolvers;
			CollectionResolvers = collectionResolvers;
		}

		public IEnumerable<TypeName> GetCapableServiceTypes()
		{
			return Resolvers.Cast<IServiceProvider>()
				.Concat(CollectionResolvers)
				.SelectMany(x => x.GetCapableServiceTypes());
		}
	}
}
