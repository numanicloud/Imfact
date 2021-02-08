using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;

namespace Deptorygen2.Core.Syntaxes
{
	internal record CollectionResolverSyntax(string MethodName,
		TypeName CollectionType,
		ParameterSyntax[] Parameters,
		ResolutionSyntax[] Resolutions) : IServiceConsumer, IServiceProvider
	{
		public TypeName ElementType => CollectionType.TypeArguments[0];


		public IEnumerable<TypeName> GetRequiredServiceTypes()
		{
			return Resolutions.SelectMany(x => x.Dependencies)
				.Except(Parameters.Select(x => x.TypeName));
		}

		public IEnumerable<TypeName> GetCapableServiceTypes()
		{
			yield return CollectionType;
		}
	}
}
