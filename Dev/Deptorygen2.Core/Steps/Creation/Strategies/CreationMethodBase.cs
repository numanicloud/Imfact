using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Instantiation.CreationMethods
{
	internal abstract class CreationMethodBase<T> : IInstantiationCoder
	{
		private readonly Dictionary<TypeName, T> _resolutionSource;
		private BooleanDisposable _requestingDisposable;

		protected CreationMethodBase(GenerationSemantics semantics)
		{
			_resolutionSource = GetSource(semantics)
				.Select(x => (type: GetTypeInfo(x), source: x))
				.ToDictionary(x => x.type, x => x.source);

			_requestingDisposable = new BooleanDisposable();
			_requestingDisposable.Dispose();
		}

		public string? GetCode(InstantiationRequest request, IInstantiationResolver resolver)
		{
			if (!_requestingDisposable.IsDisposed
				|| _resolutionSource.GetValueOrDefault(request.TypeToResolve) is not {} resolution)
			{
				return null;
			}

			return GetCreationCode(resolution, request.GivenParameters, resolver);
		}

		protected abstract string GetCreationCode(T resolution,
			GivenParameter[] given,
			IInstantiationResolver resolver);

		protected abstract IEnumerable<T> GetSource(GenerationSemantics semantics);

		protected abstract TypeName GetTypeInfo(T source);

		protected string MethodInvocation(IResolverSemantics resolver,
			GivenParameter[] given,
			IInstantiationResolver injector)
		{
			var request = new MultipleInstantiationRequest(
				resolver.Parameters.Select(x => x.TypeName).ToArray(), given);

				return $"{resolver.MethodName}({GetArgList(request, injector)})";
			
		}

		protected string GetArgList(MultipleInstantiationRequest request,
			IInstantiationResolver resolver)
		{
			// GetInjectionsの途中でこのCreation自身に戻ってくることがあるが、
			// それは無限ループを起こすのでBooleanDisposableを使って弾く
			using (_requestingDisposable = new BooleanDisposable())
			{
				return resolver.GetInjections(request).Join(", ");
			}
		}
	}
}
