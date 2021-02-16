using System.Collections.Generic;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record Factory(FactoryCommon Common,
		Delegation[] Delegations,
		Inheritance[] Inheritances,
		EntryResolver[] EntryResolvers) : IFactorySemantics, ISemanticsNode
	{
		public TypeNode Type => Common.Type;
		public Resolver[] Resolvers => Common.Resolvers;
		public MultiResolver[] MultiResolvers => Common.MultiResolvers;

		public IEnumerable<ISemanticsNode> Traverse()
		{
			yield return Common;

			foreach (var delegation in Delegations)
			{
				yield return delegation;
			}

			foreach (var inheritance in Inheritances)
			{
				yield return inheritance;
			}

			foreach (var entryResolver in EntryResolvers)
			{
				yield return entryResolver;
			}
		}
	}
}
