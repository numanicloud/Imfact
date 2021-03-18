using System.Collections.Generic;
using System.Linq;
using Imfact.Entities;
using Imfact.Steps.Semanticses;
using Imfact.Steps.Semanticses.Interfaces;
using Imfact.Steps.Semanticses.Records;
using Imfact.Utilities;

namespace Imfact.Steps.Dependency.Components
{
	internal sealed class UsingRule
	{
		public string[] Extract(SemanticsResult semantics, InjectionResult injection, DisposableInfo disposableInfo)
		{
			return Stage1(semantics, injection, disposableInfo)
				.Distinct()
				.OrderBy(x => x)
				.ToArray();
		}

		private IEnumerable<string> Stage1(SemanticsResult semantics, InjectionResult injection,
			DisposableInfo disposableInfo)
		{
			yield return semantics.Factory.Type.FullNamespace;
			yield return "System.Collections.Generic";
			yield return "System.ComponentModel";
			yield return "Imfact.Annotations";

			if (disposableInfo.HasDisposable)
			{
				yield return "System";
			}

			if (disposableInfo.HasAsyncDisposable)
			{
				yield return "System.Threading.Tasks";
			}

			foreach (var ns in Stage2(semantics, injection).SelectMany(x => x))
			{
				yield return ns;
			}
		}

		private IEnumerable<IEnumerable<string>> Stage2(SemanticsResult semantics, InjectionResult injection)
		{
			static IEnumerable<TypeAnalysis> GetNestedTypes(TypeAnalysis root)
			{
				yield return root;

				foreach (var argument in root.TypeArguments)
				{
					foreach (var type in GetNestedTypes(argument))
					{
						yield return type;
					}
				}
			}

			var factory = semantics.Factory;
			yield return factory.Delegations.Select(x => x.Type.FullNamespace);
			yield return factory.Resolvers
				.Concat<IResolverSemantics>(factory.MultiResolvers)
				.SelectMany(m =>
				{
					return m.Parameters
						.Concat<IVariableSemantics>(m.Hooks)
						.Select(x => x.Type)
						.Concat(m.Resolutions.Select(x => x.TypeName))
						.Append(m.ReturnType)
						.SelectMany(GetNestedTypes)
						.Select(x => x.FullNamespace);
				});
			yield return factory.Resolvers.Select(x => x.ReturnTypeResolution)
				.FilterNull()
				.Select(x => x.TypeName.FullNamespace);
			yield return injection.Dependencies.Select(x => x.TypeName.FullNamespace);
		}
	}
}
