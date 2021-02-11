using System.Collections.Generic;
using Deptorygen2.Core.Steps.Definitions;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Instantiation.CreationMethods
{
	internal class FieldCreation : CreationMethodBase<DependencyDefinition>
	{
		public FieldCreation(FactorySemantics factory, DependencyDefinition[] fields) : base(factory, fields)
		{
		}

		public override InstantiationMethod Method => InstantiationMethod.Field;

		protected override string GetCreationCode(DependencyDefinition resolution, ResolverParameterDefinition[] given,
			IInstantiationResolver resolver)
		{
			return resolution.FieldName;
		}

		protected override IEnumerable<DependencyDefinition> GetSource(FactorySemantics factory, DependencyDefinition[] fields)
		{
			return fields;
		}

		protected override TypeName GetTypeInfo(DependencyDefinition source)
		{
			return source.FieldType;
		}
	}
}
