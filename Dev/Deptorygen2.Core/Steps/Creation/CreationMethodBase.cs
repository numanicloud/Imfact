using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Creation.Strategies
{
	internal abstract class CreationMethodBase<T> : ICreationStrategy
		where T : notnull
	{
		private readonly Dictionary<TypeName, T> _resolutionSource;

		protected CreationMethodBase(GenerationSemantics semantics)
		{
			_resolutionSource = GetSource(semantics)
				.Select(x => (type: GetTypeInfo(x), source: x))
				.ToDictionary(x => x.type, x => x.source);
		}

		public string? GetCode(CreationRequest request, ICreationAggregator aggregator)
		{
			if (_resolutionSource.GetValueOrDefault(request.TypeToResolve) is not { } resolution)
			{
				return null;
			}

			return GetCreationCode(resolution, request.GivenParameters, aggregator);
		}

		protected abstract string GetCreationCode(T resolution,
			GivenParameter[] given,
			ICreationAggregator aggregator);

		protected abstract IEnumerable<T> GetSource(GenerationSemantics semantics);

		protected abstract TypeName GetTypeInfo(T source);

		protected string MethodInvocation(IResolverSemantics resolver,
			GivenParameter[] given,
			ICreationAggregator injector)
		{
			var request = new MultipleCreationRequest(
				resolver.Parameters.Select(x => x.TypeName).ToArray(), given, false);

			return $"{resolver.MethodName}({GetArgList(request, injector)})";
		}

		protected string GetArgList(MultipleCreationRequest request,
			ICreationAggregator aggregator)
		{
			return aggregator.GetInjections(request).Join(", ");
		}
	}
}
