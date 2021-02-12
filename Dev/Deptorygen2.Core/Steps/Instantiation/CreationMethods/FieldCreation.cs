using System.Collections.Generic;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Instantiation.CreationMethods
{
	internal class FieldCreation : CreationMethodBase<DependencySemantics>
	{
		public FieldCreation(GenerationSemantics semantics) : base(semantics)
		{
		}

		public override InstantiationMethod Method => InstantiationMethod.Field;

		protected override string GetCreationCode(DependencySemantics resolution, GivenParameter[] given,
			IInstantiationResolver resolver)
		{
			return resolution.FieldName;
		}

		protected override IEnumerable<DependencySemantics> GetSource(GenerationSemantics semantics)
		{
			return semantics.Dependencies;
		}

		protected override TypeName GetTypeInfo(DependencySemantics source)
		{
			return source.TypeName;
		}
	}
}
