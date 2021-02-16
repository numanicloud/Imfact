using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Annotations;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Aspects.Nodes;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using NacHelpers.Extensions;
using Attribute = Deptorygen2.Core.Steps.Aspects.Nodes.Attribute;
using Parameter = Deptorygen2.Core.Steps.Semanticses.Nodes.Parameter;

namespace Deptorygen2.Core.Steps.Semanticses
{
	class SemanticsAggregator2
	{
		private readonly IAnalysisContext _context;

		public SemanticsAggregator2(IAnalysisContext context)
		{
			_context = context;
		}

		public Generation? Aggregate(Class @class)
		{
			var factory = AggregateFactory(@class);
			if (factory is null)
			{
				return null;
			}

			var dependency = Dependency.FromFactory(factory);
			var namespaces = AggregateNamespaces(factory, dependency).ToArray();
			var disposableInfo = DisposableInfo.Aggregate(factory, dependency);
			return new Generation(namespaces, factory, dependency, disposableInfo);
		}

		private Factory? AggregateFactory(Class @class)
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
				AggregateResolver(methods),
				AggregateMultiResolver(methods));
		}

		private EntryResolver[] AggregateEntryResolvers(FactoryCommon factory)
		{
			return factory.Resolvers.Cast<IResolverSemantics>()
				.Concat(factory.MultiResolvers)
				.Select(EntryResolver.FromResolver)
				.ToArray();
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

		private MultiResolver[] AggregateMultiResolver(Method[] methods)
		{
			return methods.Where(x => x.IsCollectionResolver())
				.Select(m => new MultiResolver(GetMethodCommon(m)))
				.ToArray();
		}

		private Resolver[] AggregateResolver(Method[] getMethods)
		{
			return getMethods.Where(x => x.IsSingleResolver())
				.Select(m =>
				{
					var ret = m.GetReturnType(_context) is { } t
						? Resolution.Build(t, _context)
						: null;
					var abstraction = GetMethodCommon(m);

					return new Resolver(abstraction, ret);
				}).ToArray();
		}

		private ResolverCommon GetMethodCommon(Method method)
		{
			var parameters = method.GetParameters();
			var attributes = method.GetAttributes();
			var ctx = new Parameter(TypeNode.FromRuntime(typeof(ResolutionContext)), "context");

			return new ResolverCommon(
				method.Symbol.DeclaredAccessibility,
				TypeNode.FromSymbol(method.Symbol.ReturnType),
				"__" + method.Symbol.Name,
				AggregateParameter(parameters).Append(ctx).ToArray(),
				AggregateResolution(attributes),
				AggregateHooks(attributes, method));
		}

		private Hook[] AggregateHooks(Attribute[] attributes, Method method)
		{
			return attributes.Select(x => Hook.Build(x, _context, method))
				.FilterNull()
				.ToArray();
		}

		private Parameter[] AggregateParameter(Aspects.Nodes.Parameter[] parameters)
		{
			return parameters.Select(x => Parameter.Build(x, _context))
				.FilterNull()
				.ToArray();
		}

		private Resolution[] AggregateResolution(Attribute[] attributes)
		{
			return attributes.Select(x => Resolution.Build(x, _context))
				.FilterNull()
				.ToArray();
		}

		private static IEnumerable<string> AggregateNamespaces(Factory semantics, Dependency[] dependencies)
		{
			return semantics.TraverseDeep().OfType<INamespaceClaimer>()
				.Concat(dependencies)
				.SelectMany(x => x.GetRequiredNamespaces())
				.Except(semantics.Type.FullNamespace.WrapByArray())
				.Append("System.ComponentModel")
				.Distinct();
		}
	}
}
