using System;
using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Steps.Semanticses;
using NacHelpers.Extensions;
using static Deptorygen2.Core.Steps.Instantiation.InstantiationMethod;
using static Deptorygen2.Core.Steps.Instantiation.InstantiationRuleFactory;

namespace Deptorygen2.Core.Steps.Instantiation
{
	internal interface IInstantiationResolver
	{
		string? GetInjection(InstantiationRequest request);
		IEnumerable<string> GetInjections(MultipleInstantiationRequest request);
	}

	internal class InstantiationResolver : IInstantiationResolver
	{
		private readonly IInstantiationCoder[] _instantiationCoders;

		public InstantiationResolver(GenerationSemantics semantics)
		{
			_instantiationCoders = GetCreations(semantics)
				.OrderBy(x => (int)x.Method).ToArray();
		}

		public string? GetInjection(InstantiationRequest request)
		{
			var multiple = new MultipleInstantiationRequest(
				request.TypeToResolve.WrapByArray(),
				request.GivenParameters,
				request.Exclude);
			return GetInjections(multiple).FirstOrDefault();
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
					yield return GetInjectionWithoutParameter(request.OfType(type)) ?? throw new Exception();
				}
			}
		}
		
		private string? GetInjectionWithoutParameter(InstantiationRequest request)
		{
			return _instantiationCoders.Where(x => (x.Method & request.Exclude) == None)
				.Select(x => x.GetCode(request, this))
				.FirstOrDefault(x => x is not null);
		}
	}
}