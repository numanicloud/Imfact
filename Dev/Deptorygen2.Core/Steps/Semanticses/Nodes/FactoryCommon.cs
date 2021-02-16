using System.Collections.Generic;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;
using IServiceProvider = Deptorygen2.Core.Interfaces.IServiceProvider;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record FactoryCommon(TypeNode Type,
		Resolver[] Resolvers,
		MultiResolver[] MultiResolvers) : IFactorySemantics, INamespaceClaimer, IServiceProvider
	{
		public IEnumerable<ISemanticsNode> Traverse()
		{
			yield return Type;

			foreach (var resolver in Resolvers)
			{
				yield return resolver;
			}

			foreach (var multiResolver in MultiResolvers)
			{
				yield return multiResolver;
			}
		}

		public IEnumerable<TypeNode> GetCapableServiceTypes()
		{
			yield return Type;
		}

		public IEnumerable<string> GetRequiredNamespaces()
		{
			yield return Type.FullNamespace;
		}
	}
}
