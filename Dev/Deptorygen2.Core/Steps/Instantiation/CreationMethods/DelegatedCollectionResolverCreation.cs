using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Steps.Definitions;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;
using Source = System.ValueTuple<Deptorygen2.Core.Steps.Semanticses.DelegationSemantics,
	Deptorygen2.Core.Steps.Semanticses.CollectionResolverSemantics>;

namespace Deptorygen2.Core.Steps.Instantiation.CreationMethods
{
	internal class DelegatedCollectionResolverCreation : CreationMethodBase<Source>
	{
		public DelegatedCollectionResolverCreation(SourceCodeDefinition definition) : base(definition)
		{
		}

		public override InstantiationMethod Method =>
			InstantiationMethod.DelegatedCollectionResolver;

		protected override string GetCreationCode(Source resolution,
			ResolverParameterDefinition[] given, IInstantiationResolver resolver)
		{
			var invocation = MethodInvocation(resolution.Item2, given, Method, resolver);
			return $"{resolution.Item1.PropertyName}.{invocation}";
		}

		protected override IEnumerable<Source> GetSource(SourceCodeDefinition definition)
		{
			return factory.Delegations.SelectMany(x => x.CollectionResolvers.Select(y => (x, y)));
		}

		protected override TypeName GetTypeInfo(Source source)
		{
			return source.Item2.CollectionType;
		}
	}
}
