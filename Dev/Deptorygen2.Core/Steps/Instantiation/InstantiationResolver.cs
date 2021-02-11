using System;
using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Steps.Definitions;
using Deptorygen2.Core.Steps.Semanticses;
using static Deptorygen2.Core.Steps.Instantiation.InstantiationMethod;
using static Deptorygen2.Core.Steps.Instantiation.InstantiationRuleFactory;

namespace Deptorygen2.Core.Steps.Instantiation
{
	internal interface IInstantiationResolver
	{
		IEnumerable<string> GetInjections(MultipleInstantiationRequest request);
	}

	internal class InstantiationResolver : IInstantiationResolver
	{
		private readonly IInstantiationCoder[] _instantiationCoders;

		public InstantiationResolver(FactorySemantics factory, DependencyDefinition[] fields)
		{
			_instantiationCoders = GetCreations(factory, fields)
				.OrderBy(x => (int)x.Method).ToArray();
		}

		public IEnumerable<string> GetInjections(MultipleInstantiationRequest request)
		{
			var table = request.GivenParameters.GroupBy(x => x.Type)
				.ToDictionary(x => x.Key, x => x.ToList());

			foreach (var type in request.TypeToResolve)
			{
				if (table.GetValueOrDefault(type) is { } onType)
				{
					var result = onType[0].Name;
					onType.RemoveAt(0);
					yield return result;
				}
				else
				{
					yield return GetInjection(request.OfType(type)) ?? throw new Exception();
				}
			}
		}
		
		private string? GetInjection(InstantiationRequest request)
		{
			return _instantiationCoders.Where(x => (x.Method & request.Exclude) == None)
				.Select(x => x.GetCode(request, this))
				.FirstOrDefault(x => x is not null);
		}
	}
}