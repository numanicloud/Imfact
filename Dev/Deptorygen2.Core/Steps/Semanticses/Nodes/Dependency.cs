using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record Dependency(TypeName TypeName, string FieldName) : INamespaceClaimer
	{
		public static Dependency[] FromFactory(Factory semantics)
		{
			var consumers = semantics.TraverseDeep().OfType<IServiceConsumer>()
				.SelectMany(x => x.GetRequiredServiceTypes())
				.Distinct();

			var providers = semantics.TraverseDeep().OfType<IServiceProvider>()
				.SelectMany(x => x.GetCapableServiceTypes())
				.Distinct();

			return consumers.Except(providers)
				.Select(t => new Dependency(t, "_" + t.Name.ToLowerCamelCase()))
				.ToArray();
		}

		public IEnumerable<ISemanticsNode> Traverse()
		{
			yield return this;
		}

		public IEnumerable<string> GetRequiredNamespaces()
		{
			yield return TypeName.FullNamespace;
		}
	}
}
