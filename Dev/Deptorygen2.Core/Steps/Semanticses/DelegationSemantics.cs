using System;
using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Aggregation;
using Deptorygen2.Core.Utilities;
using IServiceProvider = Deptorygen2.Core.Interfaces.IServiceProvider;

namespace Deptorygen2.Core.Steps.Semanticses
{
	internal record DelegationSemantics(string PropertyName,
		TypeName TypeName,
		ResolverSemantics[] Resolvers,
		CollectionResolverSemantics[] CollectionResolvers) : IServiceProvider, INamespaceClaimer
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

		public static DelegationSemantics? Build(PropertyToAnalyze property,
			Func<Partial, DelegationSemantics> completion)
		{
			if (!property.IsDelegation())
			{
				return null;
			}

			var partial = new Partial(property.Symbol.Name,
				TypeName.FromSymbol(property.Symbol.Type));

			return completion(partial);
		}

		public record Partial(string PropertyName,
			TypeName TypeName)
		{
			public DelegationSemantics Complete(ResolverSemantics[] resolvers,
				CollectionResolverSemantics[] collectionResolvers)
			{
				return new DelegationSemantics(PropertyName, TypeName,
					resolvers, collectionResolvers);
			}
		}
	}
}
