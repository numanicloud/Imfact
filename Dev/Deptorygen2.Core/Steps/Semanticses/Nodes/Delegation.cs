using System.Collections.Generic;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record Delegation(FactoryCommon Common, string PropertyName)
		: INamespaceClaimer, IFactorySemantics
	{
		public IEnumerable<ISemanticsNode> Traverse()
		{
			yield return this;
			yield return Common;
		}

		public IEnumerable<string> GetRequiredNamespaces()
		{
			yield break;
		}

		public TypeName Type => Common.Type;

		public Resolver[] Resolvers => Common.Resolvers;

		public MultiResolver[] MultiResolvers => Common.MultiResolvers;
	}
}
