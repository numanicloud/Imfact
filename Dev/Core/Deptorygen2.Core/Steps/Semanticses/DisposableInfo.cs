using System.Linq;
using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Semanticses
{
	internal record DisposableInfo(bool HasDisposable, bool HasAsyncDisposable)
	{
		public static DisposableInfo Aggregate(Factory factory, Dependency[] dependencies)
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
