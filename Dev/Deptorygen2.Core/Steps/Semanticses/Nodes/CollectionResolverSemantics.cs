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
	internal record CollectionResolverSemantics(string MethodName,
		TypeName ReturnType,
		ParameterSemantics[] Parameters,
		ResolutionSemantics[] Resolutions,
		Accessibility Accessibility) : IServiceConsumer, IServiceProvider, INamespaceClaimer, IResolverSemantics
	{
		public TypeName ElementType => ReturnType.TypeArguments[0];


		public IEnumerable<TypeName> GetRequiredServiceTypes()
		{
			return Resolutions.SelectMany(x => x.Dependencies)
				.Except(Parameters.Select(x => x.TypeName));
		}

		public IEnumerable<TypeName> GetCapableServiceTypes()
		{
			yield return ReturnType;
		}

		public IEnumerable<string> GetRequiredNamespaces()
		{
			yield return "System.Collections.Generic";
			yield return ReturnType.TypeArguments[0].FullNamespace;

			foreach (var parameter in Parameters)
			{
				yield return parameter.TypeName.FullNamespace;
			}

			foreach (var resolution in Resolutions)
			{
				yield return resolution.TypeName.FullNamespace;
			}
		}

		public static Builder<MethodToAnalyze,
			(ParameterSemantics[], ResolutionSemantics[]),
			CollectionResolverSemantics>? GetBuilder(MethodToAnalyze method)
		{
			if (!method.IsCollectionResolver())
			{
				return null;
			}

			return new(method, tuple => new CollectionResolverSemantics(
				method.Symbol.Name,
				TypeName.FromSymbol(method.Symbol.ReturnType),
				tuple.Item1,
				tuple.Item2,
				method.Symbol.DeclaredAccessibility));
		}

		public static CollectionResolverSemantics? Build(MethodToAnalyze method,
			Func<Partial, CollectionResolverSemantics> completion)
		{
			if (!method.IsCollectionResolver())
			{
				return null;
			}

			var partial = new Partial(method.Symbol.Name,
				TypeName.FromSymbol(method.Symbol.ReturnType),
				method.Symbol.DeclaredAccessibility);

			return completion(partial);
		}

		public record Partial(string MethodName,
			TypeName CollectionType,
			Accessibility Accessibility)
		{
			public CollectionResolverSemantics Complete(ParameterSemantics[] parameters,
				ResolutionSemantics[] resolutions)
			{
				return new CollectionResolverSemantics(MethodName, CollectionType, parameters,
					resolutions, Accessibility);
			}
		}
	}
}
