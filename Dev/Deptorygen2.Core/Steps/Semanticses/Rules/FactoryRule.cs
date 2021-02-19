using System.Linq;
using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Steps.Aspects;
using Deptorygen2.Core.Steps.Semanticses.Nodes;

namespace Deptorygen2.Core.Steps.Semanticses.Rules
{
	internal sealed class FactoryRule
	{
		private readonly ResolverRule _resolverRule;

		public FactoryRule(ResolverRule resolverRule)
		{
			_resolverRule = resolverRule;
		}

		public Factory ExtractFactory(ClassAspect aspect)
		{
			var common = GetFactoryCommon(aspect.Type, aspect.Methods);
			return new Factory(common,
				ExtractDelegations(aspect.Properties),
				ExtractInheritance(aspect.BaseClasses),
				ExtractEntryResolvers(aspect));
		}

		private FactoryCommon GetFactoryCommon(TypeNode type, MethodAspect[] methods)
		{
			return new FactoryCommon(type,
				_resolverRule.ExtractResolver(methods),
				_resolverRule.ExtractMultiResolver(methods));
		}

		private Inheritance[] ExtractInheritance(ClassAspect[] baseClasses)
		{
			return baseClasses.Select(x => new Inheritance(GetFactoryCommon(x.Type, x.Methods)))
				.ToArray();
		}

		private Delegation[] ExtractDelegations(PropertyAspect[] properties)
		{
			return properties.Select(x =>
				new Delegation(GetFactoryCommon(x.Type, x.MethodsInType), x.Name))
				.ToArray();
		}

		private EntryResolver[] ExtractEntryResolvers(ClassAspect aspect)
		{
			return aspect.Methods.Select(x =>
			{
				var parameters = x.Parameters
					.Select(y => new Nodes.Parameter(y.Type, y.Name))
					.ToArray();

				return new EntryResolver(x.Name,
					x.ReturnType.Type.Node,
					parameters,
					x.Accessibility);
			}).ToArray();
		}
	}
}
