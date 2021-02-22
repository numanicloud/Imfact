using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Creation.Strategies.Template
{
	interface IFactorySource<T> where T : IFactorySemantics
	{
		string GetVariableName(T source);
		T[] GetDelegationSource();
	}
	class RootFactorySource : IFactorySource<Factory>
	{
		private readonly SemanticsRoot _semantics;

		public RootFactorySource(SemanticsRoot semantics)
		{
			this._semantics = semantics;
		}

		public string GetVariableName(Factory source) => "this";

		public Factory[] GetDelegationSource()
			=> _semantics.Factory.WrapByArray();
	}

	class DelegationSource : IFactorySource<Delegation>
	{
		private readonly SemanticsRoot _semantics;

		public DelegationSource(SemanticsRoot semantics)
		{
			this._semantics = semantics;
		}

		public string GetVariableName(Delegation source) => source.PropertyName;
		public Delegation[] GetDelegationSource()
			=> _semantics.Factory.Delegations;
	}

	class InheritanceSource : IFactorySource<Inheritance>
	{
		private readonly SemanticsRoot _semantics;

		public InheritanceSource(SemanticsRoot semantics)
		{
			this._semantics = semantics;
		}

		public string GetVariableName(Inheritance source) => "base";

		public Inheritance[] GetDelegationSource()
			=> _semantics.Factory.Inheritances;
	}
}