using System.Collections.Generic;
using System.Linq;
using Imfact.Entities;
using Imfact.Steps.Dependency.Components;
using Imfact.Steps.Dependency.Interfaces;
using Imfact.Steps.Semanticses.Interfaces;
using Imfact.Utilities;

namespace Imfact.Steps.Dependency.Strategies
{
	internal class FactoryItselfStrategy<TFactory> : IExpressionStrategy
		where TFactory : IFactorySemantics
	{
		private readonly Dictionary<TypeId, string> _map;

		public FactoryItselfStrategy(IFactorySource<TFactory> factorySource)
		{
			var roots = from p in factorySource.GetDelegationSource()
						select (p.Type, var: factorySource.GetVariableName(p));

			var impls = from p in factorySource.GetDelegationSource() 
				from q in p.Implementations
				select (q.Type, var: factorySource.GetVariableName(p));

			var grouped = from p in roots.Concat(impls)
						  group p by p.Type.Id;

			_map = grouped.ToDictionary(x => x.Key, x => x.First().var);
		}

		public ICreationNode? GetExpression(CreationContext context)
		{
			if (_map.GetValueOrDefault(context.TypeToResolve[0].Id) is not {} resolution)
			{
				return null;
			}

			return new Variable(resolution);
		}
	}
}
