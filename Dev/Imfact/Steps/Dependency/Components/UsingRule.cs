using System;
using System.Collections.Generic;
using System.Linq;
using Imfact.Entities;
using Imfact.Steps.Semanticses;
using Imfact.Steps.Semanticses.Interfaces;
using Imfact.Utilities;

namespace Imfact.Steps.Dependency.Components
{
	internal sealed class UsingRule
	{
		public string[] Extract(SemanticsRoot semantics, InjectionResult injection, DisposableInfo disposableInfo)
		{
			return Stage1(semantics, injection, disposableInfo)
				.Distinct()
				.OrderBy(x => x)
				.ToArray();
		}

		private IEnumerable<string> Stage1(SemanticsRoot semantics, InjectionResult injection,
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

		private IEnumerable<IEnumerable<string>> Stage2(SemanticsRoot semantics, InjectionResult injection)
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
					var pp = m.Parameters.SelectMany(x => GetNestedTypes(x.Type));
					var hh = m.Hooks.SelectMany(x => GetNestedTypes(x.HookType));
					var rr = m.Resolutions.SelectMany(x => GetNestedTypes(x.TypeName));
					var rt = GetNestedTypes(m.ReturnType);
					return pp.Concat(hh).Concat(rr).Concat(rt)
						.Select(x => x.FullNamespace);
				});
			yield return factory.Resolvers.Select(x => x.ReturnTypeResolution)
				.FilterNull()
				.Select(x => x.TypeName.FullNamespace);
			yield return injection.Dependencies.Select(x => x.TypeName.FullNamespace);
		}
	}
}
