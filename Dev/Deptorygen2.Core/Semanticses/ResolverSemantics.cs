using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Syntaxes
{
	internal record ResolverSemantics(string MethodName,
		TypeName ReturnTypeName,
		ResolutionSemantics? ReturnTypeResolution,
		ResolutionSemantics[] Resolutions,
		ParameterSemantics[] Parameters,
		Accessibility Accessibility) : IServiceConsumer, IServiceProvider, INamespaceClaimer
	{
		public IEnumerable<TypeName> GetRequiredServiceTypes()
		{
			return ReturnTypeResolution.AsEnumerable()
				.Concat(Resolutions)
				.SelectMany(x => x.Dependencies)
				.Except(Parameters.Select(x => x.TypeName));
		}

		public IEnumerable<TypeName> GetCapableServiceTypes()
		{
			yield return ReturnTypeName;
		}

		public IEnumerable<string> GetRequiredNamespaces()
		{
			yield return ReturnTypeName.FullNamespace;
			foreach (var parameter in Parameters)
			{
				yield return parameter.TypeName.FullNamespace;
			}

			if (Resolutions.Any())
			{
				yield return Resolutions[0].TypeName.FullNamespace;
			}
		}
	}
}
