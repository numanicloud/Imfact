using System.Collections.Generic;
using Deptorygen2.Core.Steps.Definitions;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Instantiation.CreationMethods
{
	internal class CollectionResolverCreation : CreationMethodBase<CollectionResolverDefinition>
	{
		public CollectionResolverCreation(SourceCodeDefinition definition) : base(definition)
		{
		}

		public override InstantiationMethod Method => InstantiationMethod.CollectionResolver;

		protected override string GetCreationCode(CollectionResolverDefinition resolution, ResolverParameterDefinition[] given,
			IInstantiationResolver resolver)
		{
			return MethodInvocation(resolution, given, Method, resolver);
		}

		protected override IEnumerable<CollectionResolverDefinition> GetSource(SourceCodeDefinition definition)
		{
			return definition.Factory.CollectionResolvers;
		}

		protected override TypeName GetTypeInfo(CollectionResolverDefinition source)
		{
			return source.ReturnType;
		}
	}
}
