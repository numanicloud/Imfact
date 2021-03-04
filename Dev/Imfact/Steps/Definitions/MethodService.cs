using System.Linq;
using Imfact.Entities;
using Imfact.Steps.Dependency;
using Imfact.Steps.Semanticses.Interfaces;

namespace Imfact.Steps.Definitions
{
	internal sealed class MethodService
	{
		private readonly DependencyRoot _dependency;
		private Hook[]? _cache;

		public MethodService(DependencyRoot dependency)
		{
			_dependency = dependency;
		}

		public Hook[] ExtractHooks()
		{
			return _cache ??= _dependency.Resolvers
				.Concat<IResolverSemantics>(_dependency.MultiResolvers)
				.SelectMany(x => x.Hooks)
				.Select(x => new Hook(new Type(x.HookType), x.FieldName))
				.ToArray();
		}

		public Parameter BuildParameter(TypeNode type, string name)
		{
			return new(new Type(type), name, false);
		}
	}
}
