using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Steps.Definitions;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Instantiation.CreationMethods
{
	internal class DelegatedResolverCreation : CreationMethodBase<(DelegationDefinition, ResolverDefinition)>
	{
		public DelegatedResolverCreation(SourceCodeDefinition definition) : base(definition)
		{
		}

		public override InstantiationMethod Method => InstantiationMethod.DelegatedResolver;

		protected override string GetCreationCode((DelegationDefinition, ResolverDefinition) resolution,
			ResolverParameterDefinition[] given,
			IInstantiationResolver resolver)
		{
			var invocation = MethodInvocation(resolution.Item2, given, Method, resolver);
			return $"{resolution.Item1.PropertyName}.{invocation}";
		}

		protected override IEnumerable<(DelegationDefinition, ResolverDefinition)> GetSource(SourceCodeDefinition definition)
		{
			return definition.Factory.Delegations
				.SelectMany(x => x..Select(y => (x, y)));
		}

		protected override TypeName GetTypeInfo((DelegationDefinition, ResolverDefinition) source)
		{
			return source.Item2.ReturnTypeName;
		}
	}
}
