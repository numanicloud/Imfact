using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Creation.Strategies.Template
{
	interface IFactorySource<T> where T : IFactorySemantics
	{
		string GetVariableName(T source);
		T[] GetDelegationSource(Generation semantics);
	}
	class RootFactorySource : IFactorySource<Factory>
	{
		public string GetVariableName(Factory source) => "this";

		public Factory[] GetDelegationSource(Generation semantics)
			=> semantics.Factory.WrapByArray();
	}

	class DelegationSource : IFactorySource<Delegation>
	{
		public string GetVariableName(Delegation source) => source.PropertyName;
		public Delegation[] GetDelegationSource(Generation semantics)
			=> semantics.Factory.Delegations;
	}

	class InheritanceSource : IFactorySource<Inheritance>
	{
		public string GetVariableName(Inheritance source) => "base";

		public Inheritance[] GetDelegationSource(Generation semantics)
			=> semantics.Factory.Inheritances;
	}
}