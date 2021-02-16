using Deptorygen2.Core.Steps.Semanticses.Nodes;

namespace Deptorygen2.Core.Steps.Semanticses.Interfaces
{
	internal interface IFactorySemantics
	{
		Resolver[] Resolvers { get; }
		MultiResolver[] MultiResolvers { get; }
	}
}
