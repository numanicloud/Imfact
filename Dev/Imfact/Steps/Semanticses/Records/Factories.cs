using Imfact.Entities;
using Imfact.Steps.Semanticses.Interfaces;

namespace Imfact.Steps.Semanticses.Records
{
	internal record Factory
		(FactoryCommon Common, Delegation[] Delegations,
		Inheritance[] Inheritances)
		: FactoryCommon(Common);

	internal record Delegation(FactoryCommon Common, string PropertyName)
		: FactoryCommon(Common), IVariableSemantics
	{
		public string MemberName => PropertyName;
	}

	internal record Inheritance(FactoryCommon Common, Parameter[] Parameters)
		: FactoryCommon(Common);

	internal record FactoryCommon(TypeAnalysis Type,
		Resolver[] Resolvers,
		MultiResolver[] MultiResolvers) : IFactorySemantics;
}
