using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Syntaxes
{
	internal record CollectionResolverSemantics(string MethodName,
		TypeName CollectionType,
		ParameterSemantics[] Parameters,
		ResolutionSemantics[] Resolutions,
		Accessibility Accessibility) : IServiceConsumer, IServiceProvider, INamespaceClaimer
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

		public IEnumerable<string> GetRequiredNamespaces()
		{
			yield return "System.Collections.Generic";
			yield return CollectionType.TypeArguments[0].FullNamespace;

			foreach (var parameter in Parameters)
			{
				yield return parameter.TypeName.FullNamespace;
			}

			foreach (var resolution in Resolutions)
			{
				yield return resolution.TypeName.FullNamespace;
			}
		}
	}
}
