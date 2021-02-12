using System.Collections.Generic;
using Deptorygen2.Core.Steps.Definitions;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Instantiation.CreationMethods
{
	internal class ResolverCreation : CreationMethodBase<ResolverSemantics>
	{
		public ResolverCreation(SourceCodeDefinition definition) : base(definition)
		{
		}

		public override InstantiationMethod Method => InstantiationMethod.Resolver;

		protected override string GetCreationCode(ResolverSemantics resolution, ResolverParameterDefinition[] given,
			IInstantiationResolver resolver)
		{
			return MethodInvocation(resolution, given, Method, resolver);
		}

		protected override IEnumerable<ResolverSemantics> GetSource(SourceCodeDefinition definition)
		{
			return factory.Resolvers;
		}

		protected override TypeName GetTypeInfo(ResolverSemantics source)
		{
			return source.ReturnTypeName;
		}
	}
}
