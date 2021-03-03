using Imfact.Entities;

namespace Imfact.Steps.Semanticses.Interfaces
{
	internal interface IFactorySemantics
	{
		TypeNode Type { get; }
		Resolver[] Resolvers { get; }
		MultiResolver[] MultiResolvers { get; }
	}
}
