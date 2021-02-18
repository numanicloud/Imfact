using Deptorygen2.Core.Steps.Semanticses.Interfaces;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record Delegation(FactoryCommon Common, string PropertyName)
		: IFactorySemantics
	{
		public TypeNode Type => Common.Type;

		public Resolver[] Resolvers => Common.Resolvers;

		public MultiResolver[] MultiResolvers => Common.MultiResolvers;
	}
}
