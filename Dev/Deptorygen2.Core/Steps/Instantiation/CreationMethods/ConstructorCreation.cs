using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;
using Source = Deptorygen2.Core.Steps.Semanticses.ResolutionSemantics;

namespace Deptorygen2.Core.Steps.Instantiation.CreationMethods
{
	internal class ConstructorCreation : CreationMethodBase<Source>
	{
		public ConstructorCreation(GenerationSemantics semantics) : base(semantics)
		{
		}

		public override InstantiationMethod Method => InstantiationMethod.Constructor;

		protected override string GetCreationCode(Source resolution, GivenParameter[] given,
			IInstantiationResolver resolver)
		{
			var request = new MultipleInstantiationRequest(
				resolution.Dependencies, given, Method);
			return $"new {resolution.TypeName.Name}({GetArgList(request, resolver)})";
		}

		protected override IEnumerable<Source> GetSource(GenerationSemantics semantics)
		{
			return semantics.Factory.Resolvers.SelectMany(x => x.Resolutions)
				.Concat(semantics.Factory.CollectionResolvers.SelectMany(x => x.Resolutions));
		}

		protected override TypeName GetTypeInfo(Source source)
		{
			return source.TypeName;
		}
	}
}
