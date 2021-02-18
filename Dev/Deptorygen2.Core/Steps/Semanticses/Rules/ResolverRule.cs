using System.Linq;
using Deptorygen2.Annotations;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Aspects.Nodes;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using NacHelpers.Extensions;
using Parameter = Deptorygen2.Core.Steps.Semanticses.Nodes.Parameter;

namespace Deptorygen2.Core.Steps.Semanticses
{
	internal sealed class ResolverRule
	{
		private readonly IAnalysisContext _context;

		public ResolverRule(IAnalysisContext context)
		{
			_context = context;
		}

		public MultiResolver[] ExtractMultiResolver(Method[] methods)
		{
			return methods.Where(x => x.IsCollectionResolver())
				.Select(m => new MultiResolver(GetMethodCommon(m)))
				.ToArray();
		}

		public Resolver[] ExtractResolver(Method[] getMethods)
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
	}
}
