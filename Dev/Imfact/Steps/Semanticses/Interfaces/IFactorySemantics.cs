using Imfact.Entities;
using Imfact.Steps.Semanticses.Records;

namespace Imfact.Steps.Semanticses.Interfaces;

internal interface IFactorySemantics
{
	TypeAnalysis Type { get; }
	Resolver[] Resolvers { get; }
	MultiResolver[] MultiResolvers { get; }
	Implementation[] Implementations { get; }
}