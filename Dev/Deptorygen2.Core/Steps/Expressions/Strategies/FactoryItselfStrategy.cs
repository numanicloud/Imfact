using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Steps.Creation.Strategies.Template;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Expressions.Strategies
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
