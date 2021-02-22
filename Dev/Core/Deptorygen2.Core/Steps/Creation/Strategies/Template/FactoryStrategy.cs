using System;
using System.Collections.Generic;
using System.Text;
using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Steps.Creation.Abstraction;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Steps.Semanticses.Nodes;

namespace Deptorygen2.Core.Steps.Creation.Strategies.Template
{
	internal class FactoryStrategy<TFactory> : CreationMethodBase<TFactory>
		where TFactory : IFactorySemantics
	{
		private readonly IFactorySource<TFactory> _factorySource;

		public FactoryStrategy(SemanticsRoot semantics, IFactorySource<TFactory> factorySource) : base(semantics)
		{
			this._factorySource = factorySource;
		}

		protected override string GetCreationCode(TFactory resolution, GivenParameter[] given, ICreationAggregator aggregator)
		{
			return _factorySource.GetVariableName(resolution);
		}

		protected override IEnumerable<TFactory> GetSource(SemanticsRoot semantics)
		{
			return _factorySource.GetDelegationSource();
		}

		protected override TypeRecord GetTypeInfo(TFactory source)
		{
			return source.Type.Record;
		}
	}
}
