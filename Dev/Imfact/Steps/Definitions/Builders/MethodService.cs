using System.Linq;
using Imfact.Entities;
using Imfact.Steps.Dependency;
using Imfact.Steps.Semanticses.Interfaces;

namespace Imfact.Steps.Definitions.Builders
{
	internal sealed class MethodService
	{
		private readonly DependencyResult _dependency;
		private Hook[]? _cache;

		public MethodService(DependencyResult dependency)
		{
			_dependency = dependency;
		}

		public Hook[] ExtractHooks()
		{
			return _cache ??= _dependency.Resolvers
				.Concat<IResolverSemantics>(_dependency.MultiResolvers)
				.SelectMany(x => x.Hooks)
				.Select(x => new Hook(x.HookType, x.FieldName))
				.ToArray();
		}

		public Parameter BuildParameter(TypeAnalysis type, string name)
		{
			return new(type, name, false);
		}
	}
}
