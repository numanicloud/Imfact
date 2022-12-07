using Imfact.Entities;
using Imfact.Steps.Semanticses.Interfaces;

namespace Imfact.Steps.Semanticses.Records
{
	internal record Factory
		(FactoryCommon Common, Delegation[] Delegations,
		Inheritance[] Inheritances)
		: FactoryCommon(Common);

	internal record Delegation(FactoryCommon Common, string PropertyName,
		bool HasRegisterServiceMethod, bool NeedsInitialize)
		: FactoryCommon(Common), IVariableSemantics
	{
		public string MemberName => PropertyName;
	}

	internal record Inheritance(FactoryCommon Common, Parameter[] Parameters)
		: FactoryCommon(Common);

	internal record FactoryCommon(TypeAnalysis Type,
		Resolver[] Resolvers,
		MultiResolver[] MultiResolvers,
		Implementation[] Implementations) : IFactorySemantics;

	internal record Implementation(TypeAnalysis Type);
}
