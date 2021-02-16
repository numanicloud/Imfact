using System;
using Deptorygen2.Core.Steps.Aspects.Nodes;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record Inheritance(FactoryCommon Common)
		: IServiceProvider, IFactorySemantics
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

			return null;
		}

		public TypeName Type => Common.Type;

		public Resolver[] Resolvers => Common.Resolvers;

		public MultiResolver[] MultiResolvers => Common.MultiResolvers;
	}
}
