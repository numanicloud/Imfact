using System.Collections.Generic;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record FactoryCommon(TypeNode Type,
		Resolver[] Resolvers,
		MultiResolver[] MultiResolvers) : IFactorySemantics, ISemanticsNode
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
	}
}
