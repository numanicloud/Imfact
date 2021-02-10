using System;
using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Aggregation;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using IServiceProvider = Deptorygen2.Core.Interfaces.IServiceProvider;

namespace Deptorygen2.Core.Steps.Semanticses
{
	internal record ResolverSemantics(string MethodName,
		TypeName ReturnTypeName,
		ResolutionSemantics? ReturnTypeResolution,
		ResolutionSemantics[] Resolutions,
		ParameterSemantics[] Parameters,
		Accessibility Accessibility) : IServiceConsumer, IServiceProvider, INamespaceClaimer, IResolverSemantics
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

		public static ResolverSemantics? Build(MethodToAnalyze method,
			Func<Partial, ResolverSemantics> completion)
		{
			if (!method.IsSingleResolver())
			{
				return null;
			}

			var partial = new Partial(method.Symbol.DeclaredAccessibility,
				TypeName.FromSymbol(method.Symbol.ReturnType),
				method.Symbol.Name);

			return completion(partial);
		}

		public record Partial(Accessibility Accessibility,
			TypeName ReturnTypeName,
			string MethodName)
		{
			public ResolverSemantics Complete(ResolutionSemantics? returnTypeResolution,
				ResolutionSemantics[] resolutions, ParameterSemantics[] parameters)
			{
				return new ResolverSemantics(MethodName, ReturnTypeName, returnTypeResolution,
					resolutions, parameters, Accessibility);
			}
		}
	}
}
