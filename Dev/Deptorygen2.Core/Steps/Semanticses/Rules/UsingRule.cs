using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Semanticses
{
	internal sealed class UsingRule
	{
		public string[] Extract(Factory factory, Dependency[] dependencies)
		{
			var namespaces = new IEnumerable<string>[]
			{
				factory.Type.FullNamespace.WrapByArray(),
				factory.Delegations.Select(x => x.Type.FullNamespace),
				factory.Inheritances.Select(x => x.Type.FullNamespace),
				factory.Resolvers.Cast<IResolverSemantics>()
					.Concat(factory.MultiResolvers)
					.SelectMany(m =>
					{
						var p = m.Parameters.Select(x => x.Type.FullNamespace);
						var h = m.Hooks.Select(x => x.HookType.FullNamespace);
						var r = m.Resolutions.Select(x => x.TypeName.FullNamespace);
						return p.Concat(h).Concat(r);
					}),
				factory.Resolvers.Select(x => x.ReturnTypeResolution)
					.FilterNull()
					.Select(x => x.TypeName.FullNamespace),
				dependencies.Select(x => x.TypeName.FullNamespace),
				new []
				{
					"System.Collections.Generic",
					"System.ComponentModel"
				},
			};

			return namespaces.SelectMany(x => x).Distinct().ToArray();
		}
	}
}
