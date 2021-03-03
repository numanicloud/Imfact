using System.Linq;
using Imfact.Entities;
using Imfact.Steps.Semanticses;
using Imfact.Steps.Semanticses.Interfaces;

namespace Imfact.Steps.Dependency.Components
{
	internal record DisposableInfo(bool HasDisposable, bool HasAsyncDisposable)
	{
		public static DisposableInfo Aggregate(Factory factory, Semanticses.Dependency[] dependencies)
		{
			var a = dependencies.Select(x => x.TypeName);
			var b = factory.Delegations.Select(x => x.Type);
			var c = factory.Resolvers.Cast<IResolverSemantics>()
				.Concat(factory.MultiResolvers)
				.SelectMany(x => x.Hooks)
				.Select(x => x.HookType);
			var types = a.Concat(b).Concat(c).ToArray();

			var hasDisposable = types.Any(x => x.DisposableType == DisposableType.Disposable);
			var hasAsyncDisposable =
				types.Any(x => x.DisposableType == DisposableType.AsyncDisposable);
			return new DisposableInfo(hasDisposable, hasAsyncDisposable);
		}
	}
}
