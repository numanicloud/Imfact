using System.Collections.Generic;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record Inheritance(FactoryCommon Common)
		: IFactorySemantics, ISemanticsNode
	{
		public TypeNode Type => Common.Type;
		public Resolver[] Resolvers => Common.Resolvers;
		public MultiResolver[] MultiResolvers => Common.MultiResolvers;

		public IEnumerable<ISemanticsNode> Traverse()
		{
			yield return Common;
		}
	}
}
