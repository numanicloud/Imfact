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

		public static Builder<PropertyToAnalyze,
			(ResolverSemantics[], CollectionResolverSemantics[]),
			DelegationSemantics>? GetBuilder(PropertyToAnalyze property)
		{
			if (!property.IsDelegation())
			{
				return null;
			}
			
			return new(property, tuple => new DelegationSemantics(
				property.Symbol.Name,
				Utilities.TypeName.FromSymbol(property.Symbol.Type),
				tuple.Item1,
				tuple.Item2));
		}
	}
}
