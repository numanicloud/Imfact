using System;
using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Steps.Creation.Strategies;
using Deptorygen2.Core.Steps.Semanticses;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Creation
{
	internal class CreationAggregator : ICreationAggregator
	{
		private readonly ICreationStrategy[] _instantiationCoders;

		public CreationAggregator(GenerationSemantics semantics)
		{
			_instantiationCoders = GetCreations(semantics).ToArray();
		}

		public string? GetInjection(CreationRequest request)
		{
			var multiple = new MultipleCreationRequest(
				request.TypeToResolve.WrapByArray(),
				request.GivenParameters);
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
					yield return GetInjectionWithoutParameter(request.OfType(type)) ?? throw new Exception();
				}
			}
		}
		
		private string? GetInjectionWithoutParameter(CreationRequest request)
		{
			return _instantiationCoders
				.Select(x => x.GetCode(request, this))
				.FirstOrDefault(x => x is not null);
		}

		public static IEnumerable<ICreationStrategy> GetCreations(GenerationSemantics semantics)
		{
			// この順で評価されて、最初にマッチした解決方法が使われる
			yield return new FactoryItselfCreation(semantics);
			yield return new DelegationItselfCreation(semantics);
			yield return new DelegatedResolverCreation(semantics);
			yield return new DelegatedCollectionResolverCreation(semantics);
			yield return new ResolverCreation(semantics);
			yield return new CollectionResolverCreation(semantics);
			yield return new FieldCreation(semantics);
			yield return new ConstructorCreation(semantics);
		}
	}
}