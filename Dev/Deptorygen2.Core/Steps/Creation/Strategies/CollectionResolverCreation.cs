using System.Collections.Generic;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Instantiation.CreationMethods
{
	internal class CollectionResolverCreation : CreationMethodBase<CollectionResolverSemantics>
	{
		public CollectionResolverCreation(GenerationSemantics semantics) : base(semantics)
		{
		}

		protected override string GetCreationCode(CollectionResolverSemantics resolution, GivenParameter[] given,
			IInstantiationResolver resolver)
		{
			return MethodInvocation(resolution, given, resolver);
		}

		protected override IEnumerable<CollectionResolverSemantics> GetSource(GenerationSemantics semantics)
		{
			return semantics.Factory.CollectionResolvers;
		}

		protected override TypeName GetTypeInfo(CollectionResolverSemantics source)
		{
			return source.CollectionType;
		}
	}
}
