using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using Deptorygen2.Core.Utilities;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Creation.Abstraction
{
	internal abstract class CreationMethodBase<T> : ICreationStrategy
		where T : notnull
	{
		private readonly Generation _semantics;
		private Dictionary<TypeRecord, T>? _resolutionSource;

		protected CreationMethodBase(Generation semantics)
		{
			_semantics = semantics;
		}

		public string? GetCode(CreationRequest request, ICreationAggregator aggregator)
		{
			_resolutionSource ??= GetSource(_semantics)
				.Select(x => (type: GetTypeInfo(x), source: x))
				.GroupBy(x => x.type)
				.Select(x => x.First())
				.ToDictionary(x => x.type, x => x.source);

			if (_resolutionSource.GetValueOrDefault(request.TypeToResolve.Record) is not { } resolution)
			{
				return null;
			}

			return GetCreationCode(resolution, request.GivenParameters, aggregator);
		}

		protected abstract string GetCreationCode(T resolution,
			GivenParameter[] given,
			ICreationAggregator aggregator);

		protected abstract IEnumerable<T> GetSource(Generation semantics);

		protected abstract TypeRecord GetTypeInfo(T source);

		protected string GetArgList(MultipleCreationRequest request,
			ICreationAggregator aggregator)
		{
			return aggregator.GetInjections(request).Join(", ");
		}
	}
}
