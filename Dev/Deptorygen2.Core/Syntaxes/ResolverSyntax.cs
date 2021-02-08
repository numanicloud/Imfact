using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Syntaxes
{
	class ResolverSyntax : IServiceConsumer, IServiceProvider
	{
		public string MethodName { get; }
		public TypeName ReturnTypeName { get; }
		public ResolutionSyntax? ReturnTypeResolution { get; }
		public ResolutionSyntax[] Resolutions { get; }
		public ParameterSyntax[] Parameters { get; }
		public Accessibility Accessibility { get; }

		public ResolverSyntax(string methodName,
			TypeName returnTypeName,
			ResolutionSyntax? returnTypeResolution,
			ResolutionSyntax[] resolutions,
			ParameterSyntax[] parameters,
			Accessibility accessibility)
		{
			MethodName = methodName;
			ReturnTypeName = returnTypeName;
			ReturnTypeResolution = returnTypeResolution;
			Resolutions = resolutions;
			Parameters = parameters;
			Accessibility = accessibility;
		}

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
