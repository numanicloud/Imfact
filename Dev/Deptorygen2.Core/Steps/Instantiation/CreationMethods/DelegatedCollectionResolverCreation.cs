using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Steps.Definitions;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Instantiation.CreationMethods
{
	internal class DelegatedCollectionResolverCreation : CreationMethodBase<(DelegationSemantics, CollectionResolverSemantics)>
	{
		public DelegatedCollectionResolverCreation(FactorySemantics factory, DependencyDefinition[] fields) : base(factory, fields)
		{
		}

		public override InstantiationMethod Method =>
			InstantiationMethod.DelegatedCollectionResolver;

		protected override string GetCreationCode((DelegationSemantics, CollectionResolverSemantics) resolution,
			ResolverParameterDefinition[] given, IInstantiationResolver resolver)
		{
			var invocation = MethodInvocation(resolution.Item2, given, Method, resolver);
			return $"{resolution.Item1.PropertyName}.{invocation}";
		}

		protected override IEnumerable<(DelegationSemantics, CollectionResolverSemantics)> GetSource(FactorySemantics factory, DependencyDefinition[] fields)
		{
			return factory.Delegations.SelectMany(x => x.CollectionResolvers.Select(y => (x, y)));
		}

		protected override TypeName GetTypeInfo((DelegationSemantics, CollectionResolverSemantics) source)
		{
			return source.Item2.CollectionType;
		}
	}
}
