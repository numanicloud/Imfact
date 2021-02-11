using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Steps.Definitions;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Instantiation.CreationMethods
{
	internal class ConstructorCreation : CreationMethodBase<ResolutionSemantics>
	{
		public ConstructorCreation(FactorySemantics factory, DependencyDefinition[] fields) : base(factory, fields)
		{
		}

		public override InstantiationMethod Method => InstantiationMethod.Constructor;

		protected override string GetCreationCode(ResolutionSemantics resolution, ResolverParameterDefinition[] given,
			IInstantiationResolver resolver)
		{
			var request = new MultipleInstantiationRequest(
				resolution.Dependencies, given, Method);
			return $"new {resolution.TypeName.Name}({GetArgList(request, resolver)})";
		}

		protected override IEnumerable<ResolutionSemantics> GetSource(FactorySemantics factory, DependencyDefinition[] fields)
		{
			return factory.Resolvers.SelectMany(x => x.Resolutions)
				.Concat(factory.CollectionResolvers.SelectMany(x => x.Resolutions));
		}

		protected override TypeName GetTypeInfo(ResolutionSemantics source)
		{
			return source.TypeName;
		}
	}
}
