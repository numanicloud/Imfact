using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Syntaxes
{
	internal record ResolverSyntax(string MethodName,
		TypeName ReturnTypeName,
		ResolutionSyntax? ReturnTypeResolution,
		ResolutionSyntax[] Resolutions,
		ParameterSyntax[] Parameters,
		Accessibility Accessibility) : IServiceConsumer, IServiceProvider
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
	}
}
