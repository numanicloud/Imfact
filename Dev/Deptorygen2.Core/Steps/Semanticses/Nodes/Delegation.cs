using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Aspects.Nodes;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;
using IServiceProvider = Deptorygen2.Core.Interfaces.IServiceProvider;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record Delegation(string PropertyName,
		TypeName TypeName,
		Resolver[] Resolvers,
		MultiResolver[] MultiResolvers) : IServiceProvider, INamespaceClaimer, IFactorySemantics
	{
		public IEnumerable<TypeName> GetCapableServiceTypes()
		{
			return Resolvers.Cast<IServiceProvider>()
				.Concat(MultiResolvers)
				.SelectMany(x => x.GetCapableServiceTypes());
		}

		public IEnumerable<string> GetRequiredNamespaces()
		{
			yield return TypeName.FullNamespace;
		}

		public static Builder<Property,
			(Resolver[], MultiResolver[]),
			Delegation>? GetBuilder(Property property)
		{
			if (!property.IsDelegation())
			{
				return null;
			}
			
			return new(property, tuple => new Delegation(
				property.Symbol.Name,
				Utilities.TypeName.FromSymbol(property.Symbol.Type),
				tuple.Item1,
				tuple.Item2));
		}
	}
}
