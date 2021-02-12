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

		public static Builder<MethodToAnalyze,
			(ResolutionSemantics?,
			ResolutionSemantics[],
			ParameterSemantics[]),
			ResolverSemantics>? GetBuilder(MethodToAnalyze method)
		{
			if (!method.IsSingleResolver())
			{
				return null;
			}

			return new(method, tuple => new ResolverSemantics(method.Symbol.Name,
				TypeName.FromSymbol(method.Symbol.ReturnType),
				tuple.Item1,
				tuple.Item2,
				tuple.Item3,
				method.Symbol.DeclaredAccessibility));
		}
	}
}
