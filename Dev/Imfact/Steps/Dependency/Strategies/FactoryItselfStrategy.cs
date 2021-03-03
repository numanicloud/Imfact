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
		private readonly IFactorySource<TFactory> _factorySource;
		private readonly Dictionary<TypeRecord, TFactory[]> _map;

		public FactoryItselfStrategy(IFactorySource<TFactory> factorySource)
		{
			_factorySource = factorySource;
			var grouped = from p in _factorySource.GetDelegationSource()
						  group p by p.Type.Record;
			_map = grouped.ToDictionary(x => x.Key, x => x.ToArray());
		}

		public ICreationNode? GetExpression(CreationContext context)
		{
			if (_map.GetValueOrDefault(context.TypeToResolve[0].Record) is not {} resolution)
			{
				return null;
			}

			var source = _factorySource.GetVariableName(resolution[0]);
			return new Variable(source);
		}
	}
}
