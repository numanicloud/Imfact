using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Creation.Abstraction;
using Deptorygen2.Core.Steps.Creation.Strategies;
using Deptorygen2.Core.Steps.Creation.Strategies.Template;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using Deptorygen2.Core.Utilities;
using NacHelpers.Extensions;

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
			var table = request.GivenParameters.GroupBy(x => x.Type.Record)
				.ToDictionary(x => x.Key, x => x.ToList());

			foreach (var type in request.TypeToResolve)
			{
				if (table.GetValueOrDefault(type.Record) is { } onType)
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
					// 自分自身を呼び出さないように
					if (request.IsRootRequest)
					{
						return x is not TemplateStrategy<FactoryCommon, Semanticses.Nodes.MultiResolver>
							and not TemplateStrategy<Factory, Semanticses.Nodes.Resolver>;
					}

					return true;
				})
				.Select(x => x.GetCode(request, this))
				.FirstOrDefault(x => x is not null);
		}

		public static IEnumerable<ICreationStrategy> GetCreations(Generation semantics)
		{
			var factory = new RootFactorySource(semantics);
			var delegation = new DelegationSource(semantics);
			var inheritance = new InheritanceSource(semantics);
			var resolver = new ResolverSource();
			var multiResolver = new MultiResolverSource();

			// この順で評価されて、最初にマッチした解決方法が使われる
			yield return factory.GetStrategy(semantics);
			yield return delegation.GetStrategy(semantics);
			yield return (delegation, resolver).GetStrategy(semantics);
			yield return (delegation, multiResolver).GetStrategy(semantics);
			yield return (factory, resolver).GetStrategy(semantics);
			yield return (factory, multiResolver).GetStrategy(semantics);
			yield return (inheritance, resolver).GetStrategy(semantics);
			yield return (inheritance, multiResolver).GetStrategy(semantics);
			yield return new Field(semantics);
			yield return new Constructor(semantics);
		}
	}

	static class CreationExtensions
	{
		public static FactoryStrategy<TFactory> GetStrategy<TFactory>(
			this IFactorySource<TFactory> source, Generation semantics)
			where TFactory : IFactorySemantics
		{
			return new(semantics, source);
		}

		public static TemplateStrategy<TFactory, TResolver> GetStrategy<TFactory, TResolver>(
			this (IFactorySource<TFactory>, IResolverSource<TResolver>) components,
			Generation semantics)
			where TFactory : IFactorySemantics
			where TResolver : IResolverSemantics
		{
			return new(components.Item1, components.Item2, semantics);
		}
	}
}