using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Steps.Creation.Abstraction;
using Deptorygen2.Core.Steps.Creation.Strategies;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using Deptorygen2.Core.Utilities;
using NacHelpers.Extensions;
using MultiResolver = Deptorygen2.Core.Steps.Creation.Strategies.MultiResolver;
using Resolver = Deptorygen2.Core.Steps.Creation.Strategies.Resolver;

namespace Deptorygen2.Core.Steps.Creation
{
	internal class CreationAggregator : ICreationAggregator
	{
		private readonly ICreationStrategy[] _instantiationCoders;

		public CreationAggregator(Generation semantics)
		{
			_instantiationCoders = GetCreations(semantics).ToArray();
		}

		public string? GetInjection(CreationRequest request)
		{
			var multiple = new MultipleCreationRequest(
				request.TypeToResolve.WrapByArray(),
				request.GivenParameters,
				request.IsRootRequest);
			return GetInjections(multiple).FirstOrDefault();
		}

		public IEnumerable<string> GetInjections(MultipleCreationRequest request)
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
					yield return GetInjectionWithoutParameter(request.OfType(type)) ?? "__NoResolutionFound__";
				}
			}
		}
		
		private string? GetInjectionWithoutParameter(CreationRequest request)
		{
			return _instantiationCoders
				.Where(x =>
				{
					if (request.IsRootRequest)
					{
						return x is not Resolver
							and not MultiResolver;
					}

					return true;
				})
				.Select(x => x.GetCode(request, this))
				.FirstOrDefault(x => x is not null);
		}

		public static IEnumerable<ICreationStrategy> GetCreations(Generation semantics)
		{
			// この順で評価されて、最初にマッチした解決方法が使われる
			yield return new FactoryItself(semantics);
			yield return new DelegationItself(semantics);
			yield return new DelegatedResolver(semantics);
			yield return new DelegatedMultiResolver(semantics);
			yield return new Resolver(semantics);
			yield return new MultiResolver(semantics);
			yield return new Field(semantics);
			yield return new Constructor(semantics);
		}
	}
}