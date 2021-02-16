using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Aspects.Nodes;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;
using IServiceProvider = Deptorygen2.Core.Interfaces.IServiceProvider;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record Delegation(FactoryCommon Common, string PropertyName)
		: IServiceProvider, INamespaceClaimer, IFactorySemantics
	{
		public IEnumerable<TypeName> GetCapableServiceTypes()
		{
			return Resolvers.Cast<IServiceProvider>()
				.Concat(MultiResolvers)
				.SelectMany(x => x.GetCapableServiceTypes());
		}

		public IEnumerable<string> GetRequiredNamespaces()
		{
			yield return Type.FullNamespace;
		}

		public static Builder<Property,
			(Resolver[], MultiResolver[]),
			Delegation>? GetBuilder(Property property)
		{
			if (!property.IsDelegation())
			{
				return null;
			}

			return null;
		}

		public TypeName Type => Common.Type;

		public Resolver[] Resolvers => Common.Resolvers;

		public MultiResolver[] MultiResolvers => Common.MultiResolvers;
	}
}
