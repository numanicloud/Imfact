using System.Collections.Generic;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Instantiation.CreationMethods
{
	internal class DelegationItselfCreation : CreationMethodBase<DelegationSemantics>
	{
		public DelegationItselfCreation(GenerationSemantics semantics) : base(semantics)
		{
		}

		public override InstantiationMethod Method => InstantiationMethod.DelegationItself;

		protected override string GetCreationCode(DelegationSemantics resolution,
			GivenParameter[] given,
			IInstantiationResolver resolver)
		{
			return resolution.PropertyName;
		}

		protected override IEnumerable<DelegationSemantics> GetSource(GenerationSemantics semantics)
		{
			return semantics.Factory.Delegations;
		}

		protected override TypeName GetTypeInfo(DelegationSemantics source)
		{
			return source.TypeName;
		}
	}
}
