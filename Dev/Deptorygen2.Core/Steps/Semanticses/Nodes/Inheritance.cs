using System;
using Deptorygen2.Core.Steps.Aspects.Nodes;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record Inheritance(TypeName Type,
		Resolver[] Resolvers,
		MultiResolver[] MultiResolvers) : IServiceProvider, IFactorySemantics
	{
		public object GetService(Type serviceType)
		{
			throw new NotImplementedException();
		}

		public static Builder<Class,
			(Resolver[], MultiResolver[]),
			Inheritance>? GetBuilder(Class @class)
		{
			if (!@class.IsFactory())
			{
				return null;
			}

			var t = TypeName.FromSymbol(@class.Symbol);

			return new(@class, tuple => new Inheritance(
				t, tuple.Item1, tuple.Item2));
		}
	}
}
