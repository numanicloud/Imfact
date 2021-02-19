using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Aspects.Nodes;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using Microsoft.CodeAnalysis;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Semanticses
{
	internal sealed class FactoryRule
	{
		private readonly IAnalysisContext _context;
		private readonly ResolverRule _resolverRule;

		public FactoryRule(IAnalysisContext context, ResolverRule resolverRule)
		{
			_context = context;
			_resolverRule = resolverRule;
		}

		public Factory? ExtractFactory(Class @class)
		{
			if (!@class.IsFactory())
			{
				return null;
			}

			var factoryCommon = GetFactoryCommon(@class.Symbol, @class.GetMethods(_context));

			return new Factory(
				factoryCommon,
				AggregateDelegation(@class.GetProperties(_context)),
				AggregateInheritance(@class.GetBaseClasses(_context)),
				AggregateEntryResolvers(factoryCommon));
		}

		private FactoryCommon GetFactoryCommon(INamedTypeSymbol symbol, Method[] methods)
		{
			return new FactoryCommon(
				TypeNode.FromSymbol(symbol),
				_resolverRule.ExtractResolver(methods),
				_resolverRule.ExtractMultiResolver(methods));
		}

		private Inheritance[] AggregateInheritance(Class[] getBaseClasses)
		{
			return getBaseClasses.Where(x => x.IsFactory())
				.Select(x => new Inheritance(GetFactoryCommon(x.Symbol, x.GetMethods(_context))))
				.ToArray();
		}

		private Delegation[] AggregateDelegation(Property[] getProperties)
		{
			return getProperties.Where(x => x.IsDelegation())
				.Select(p =>
				{
					if (p.Symbol.Type is not INamedTypeSymbol nts)
					{
						return null;
					}

					return new Delegation(GetFactoryCommon(nts, p.GetMethodToAnalyze()), p.Symbol.Name);
				})
				.FilterNull()
				.ToArray();
		}

		private EntryResolver[] AggregateEntryResolvers(FactoryCommon factory)
		{
			return factory.Resolvers.Cast<IResolverSemantics>()
				.Concat(factory.MultiResolvers)
				.Select(EntryResolver.FromResolver)
				.ToArray();
		}
	}
}
