using System.Collections.Generic;
using Deptorygen2.Core.Steps.Definitions;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Instantiation.CreationMethods
{
	internal class DelegationItselfCreation : CreationMethodBase<DelegationSemantics>
	{
		public DelegationItselfCreation(FactorySemantics factory, DependencyDefinition[] fields) : base(factory, fields)
		{
		}

		public override InstantiationMethod Method => InstantiationMethod.DelegationItself;

		protected override string GetCreationCode(DelegationSemantics resolution,
			ResolverParameterDefinition[] given,
			IInstantiationResolver resolver)
		{
			return resolution.PropertyName;
		}

		protected override IEnumerable<DelegationSemantics> GetSource(FactorySemantics factory, DependencyDefinition[] fields)
		{
			return factory.Delegations;
		}

		protected override TypeName GetTypeInfo(DelegationSemantics source)
		{
			return source.TypeName;
		}
	}
}
