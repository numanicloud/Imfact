using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Steps.Definitions;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Instantiation.CreationMethods
{
	internal class DelegatedResolverCreation : CreationMethodBase<(DelegationSemantics, ResolverSemantics)>
	{
		public DelegatedResolverCreation(FactorySemantics factory, DependencyDefinition[] fields) : base(factory, fields)
		{
		}

		public override InstantiationMethod Method => InstantiationMethod.DelegatedResolver;

		protected override string GetCreationCode((DelegationSemantics, ResolverSemantics) resolution,
			ResolverParameterDefinition[] given,
			IInstantiationResolver resolver)
		{
			var invocation = MethodInvocation(resolution.Item2, given, Method, resolver);
			return $"{resolution.Item1.PropertyName}.{invocation}";
		}

		protected override IEnumerable<(DelegationSemantics, ResolverSemantics)> GetSource(FactorySemantics factory, DependencyDefinition[] fields)
		{
			return factory.Delegations.SelectMany(x => x.Resolvers.Select(y => (x, y)));
		}

		protected override TypeName GetTypeInfo((DelegationSemantics, ResolverSemantics) source)
		{
			return source.Item2.ReturnTypeName;
		}
	}
}
