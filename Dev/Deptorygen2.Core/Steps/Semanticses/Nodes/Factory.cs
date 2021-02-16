using System.Collections.Generic;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Aspects.Nodes;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record Factory(FactoryCommon Common,
		Delegation[] Delegations,
		Inheritance[] Inheritances,
		EntryResolver[] EntryResolvers) : IServiceProvider, IFactorySemantics
	{
		public IEnumerable<TypeName> GetCapableServiceTypes()
		{
			yield return Type;
			foreach (var delegation in Delegations)
			{
				yield return delegation.Type;
			}
		}

		public static Builder<Class,
			(Resolver[],
			MultiResolver[],
			Delegation[],
			Inheritance[],
			EntryResolver[]),
			Factory>? GetBuilder(Class @class)
		{
			if (!@class.IsFactory())
			{
				return null;
			}

			var t = TypeName.FromSymbol(@class.Symbol);

			return null;
		}

		public TypeName Type => Common.Type;

		public Resolver[] Resolvers => Common.Resolvers;

		public MultiResolver[] MultiResolvers => Common.MultiResolvers;
	}
}
