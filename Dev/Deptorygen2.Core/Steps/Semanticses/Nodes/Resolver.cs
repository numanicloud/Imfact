using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Annotations;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Aspects.Nodes;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using IServiceProvider = Deptorygen2.Core.Interfaces.IServiceProvider;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record Resolver(string MethodName,
		TypeName ReturnType,
		Resolution? ReturnTypeResolution,
		Resolution[] Resolutions,
		Parameter[] Parameters,
		Accessibility Accessibility,
		Hook[] Hooks) : IServiceConsumer, IServiceProvider, INamespaceClaimer, IResolverSemantics
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
			yield return ReturnType;
		}

		public IEnumerable<string> GetRequiredNamespaces()
		{
			yield return ReturnType.FullNamespace;
			foreach (var parameter in Parameters)
			{
				yield return parameter.TypeName.FullNamespace;
			}

			if (Resolutions.Any())
			{
				yield return Resolutions[0].TypeName.FullNamespace;
			}
		}

		public static Builder<Method,
			(Resolution?,
			Resolution[],
			Parameter[],
			Hook[]),
			Resolver>? GetBuilder(Method method)
		{
			if (!method.IsSingleResolver())
			{
				return null;
			}
			
			var ctxType = new Parameter(TypeName.FromType(typeof(ResolutionContext)), "context");

			return new(method, tuple => new Resolver(
				"__" + method.Symbol.Name,
				TypeName.FromSymbol(method.Symbol.ReturnType),
				tuple.Item1,
				tuple.Item2,
				tuple.Item3.Append(ctxType).ToArray(),
				method.Symbol.DeclaredAccessibility,
				tuple.Item4));
		}
	}
}
