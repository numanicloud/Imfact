using System.Collections.Generic;
using Deptorygen2.Core.Steps.Definitions;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Instantiation.CreationMethods
{
	internal class DelegationItselfCreation : CreationMethodBase<DelegationDefinition>
	{
		public DelegationItselfCreation(SourceCodeDefinition definition) : base(definition)
		{
		}

		public override InstantiationMethod Method => InstantiationMethod.DelegationItself;

		protected override string GetCreationCode(DelegationDefinition resolution,
			ResolverParameterDefinition[] given,
			IInstantiationResolver resolver)
		{
			return resolution.PropertyName;
		}

		protected override IEnumerable<DelegationDefinition> GetSource(SourceCodeDefinition definition)
		{
			return definition.Factory.Delegations;
		}

		protected override TypeName GetTypeInfo(DelegationDefinition source)
		{
			return source.PropertyType;
		}
	}
}
