using System;
using System.Collections.Generic;
using System.Text;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record FactoryCommon(TypeName Type,
		Resolver[] Resolvers,
		MultiResolver[] MultiResolvers) : IFactorySemantics
	{
	}
}
