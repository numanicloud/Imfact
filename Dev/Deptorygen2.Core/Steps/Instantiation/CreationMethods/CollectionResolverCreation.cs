using System.Collections.Generic;
using Deptorygen2.Core.Steps.Definitions;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Instantiation.CreationMethods
{
	internal class CollectionResolverCreation : CreationMethodBase<CollectionResolverSemantics>
	{
		public CollectionResolverCreation(FactorySemantics factory, DependencyDefinition[] fields) : base(factory, fields)
		{
		}

		public override InstantiationMethod Method => InstantiationMethod.CollectionResolver;

		protected override string GetCreationCode(CollectionResolverSemantics resolution, ResolverParameterDefinition[] given,
			IInstantiationResolver resolver)
		{
			return MethodInvocation(resolution, given, Method, resolver);
		}

		protected override IEnumerable<CollectionResolverSemantics> GetSource(FactorySemantics factory, DependencyDefinition[] fields)
		{
			return factory.CollectionResolvers;
		}

		protected override TypeName GetTypeInfo(CollectionResolverSemantics source)
		{
			return source.CollectionType;
		}
	}
}
