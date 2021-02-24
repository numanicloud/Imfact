using Deptorygen2.Core.Entities;

namespace Deptorygen2.Core.Steps.Semanticses.Interfaces
{
	internal interface IFactorySemantics
	{
		TypeNode Type { get; }
		Resolver[] Resolvers { get; }
		MultiResolver[] MultiResolvers { get; }
	}
}
