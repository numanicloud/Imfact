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

		public override InstantiationMethod Method => InstantiationMethod.CollectionResolver;

		protected override string GetCreationCode(CollectionResolverSemantics resolution, GivenParameter[] given,
			IInstantiationResolver resolver)
		{
			return MethodInvocation(resolution, given, Method, resolver);
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
