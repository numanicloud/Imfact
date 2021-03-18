using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Imfact.Entities;
using Imfact.Steps.Definitions.Methods;
using Imfact.Steps.Definitions.Methods.Implementations;
using Imfact.Steps.Dependency;
using Imfact.Steps.Semanticses.Interfaces;
using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Definitions.Builders
{
	internal sealed class DisposeMethodBuilder
	{
		private readonly DependencyRoot _dependency;

		public DisposeMethodBuilder(DependencyRoot dependency)
		{
			_dependency = dependency;
		}

		public IEnumerable<MethodInfo> BuildDisposeMethodInfo()
		{
			var disposables = ExtractDisposables();

			var syncD = disposables
				.Where(x => x.Type.DisposableType.HasFlag(DisposableType.Disposable))
				.ToArray();
			if (syncD.Any())
			{
				yield return BuildDisposable(syncD, false);
			}

			var asyncD = disposables
				.Where(x => x.Type.DisposableType.HasFlag(DisposableType.AsyncDisposable))
				.ToArray();
			if (asyncD.Any())
			{
				yield return BuildDisposable(asyncD, true);
			}
		}

		private IVariableSemantics[] ExtractDisposables()
		{
			return _dependency.Resolvers
				.Concat<IResolverSemantics>(_dependency.MultiResolvers)
				.SelectMany(x => x.Hooks)
				.Concat<IVariableSemantics>(_dependency.Dependencies)
				.Concat(_dependency.Delegations)
				.ToArray();
		}

		private MethodInfo BuildDisposable(
			IEnumerable<IVariableSemantics> disposables,
			bool isAsync)
		{
			var methodName = isAsync ? "DisposeAsync" : "Dispose";
			var modifiers = isAsync ? new[] { "async" } : new string[0];
			var returnType = isAsync ? typeof(ValueTask) : typeof(void);

			var signature = new OrdinalSignature(Accessibility.Public,
				TypeAnalysis.FromRuntime(returnType),
				methodName, new Parameter[0], modifiers);

			var impl = new DisposeImplementation(
				disposables.Select(x => x.MemberName).ToArray(),
				isAsync);

			return new MethodInfo(signature, impl);
		}
	}
}