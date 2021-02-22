using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Expressions
{
	internal sealed class UsingRule
	{
		public string[] Extract(SemanticsRoot semantics, InjectionResult injection, DisposableInfo disposableInfo)
		{
			return Stage1(semantics, injection, disposableInfo).Distinct().ToArray();
		}

		private IEnumerable<string> Stage1(SemanticsRoot semantics, InjectionResult injection,
			DisposableInfo disposableInfo)
		{
			yield return semantics.Factory.Type.FullNamespace;
			yield return "System.Collections.Generic";
			yield return "System.ComponentModel";

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
			var factory = semantics.Factory;
			yield return factory.Delegations.Select(x => x.Type.FullNamespace);
			yield return factory.Resolvers
				.Concat<IResolverSemantics>(factory.MultiResolvers)
				.SelectMany(m =>
				{
					var p = m.Parameters.Select(x => x.Type.FullNamespace);
					var h = m.Hooks.Select(x => x.HookType.FullNamespace);
					var r = m.Resolutions.Select(x => x.TypeName.FullNamespace);
					return p.Concat(h).Concat(r);
				});
			yield return factory.Resolvers.Select(x => x.ReturnTypeResolution)
				.FilterNull()
				.Select(x => x.TypeName.FullNamespace);
			yield return injection.Dependencies.Select(x => x.TypeName.FullNamespace);
		}
	}
}
