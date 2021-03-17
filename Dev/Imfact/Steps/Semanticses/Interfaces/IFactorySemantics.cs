using Imfact.Entities;

namespace Imfact.Steps.Semanticses.Interfaces
{
	internal interface IFactorySemantics
	{
		TypeAnalysis Type { get; }
		Resolver[] Resolvers { get; }
		MultiResolver[] MultiResolvers { get; }
	}
}
