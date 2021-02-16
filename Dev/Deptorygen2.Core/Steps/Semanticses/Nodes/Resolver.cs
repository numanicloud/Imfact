using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using IServiceProvider = Deptorygen2.Core.Interfaces.IServiceProvider;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record Resolver(ResolverCommon Common, Resolution? ReturnTypeResolution)
		: IServiceConsumer, IServiceProvider, IResolverSemantics
	{
		public TypeNode ReturnType => Common.ReturnType;
		public string MethodName => Common.MethodName;
		public Parameter[] Parameters => Common.Parameters;
		public Accessibility Accessibility => Common.Accessibility;
		public Resolution[] Resolutions => Common.Resolutions;
		public Hook[] Hooks => Common.Hooks;

		public IEnumerable<TypeNode> GetRequiredServiceTypes()
		{
			return ReturnTypeResolution.AsEnumerable()
				.Concat(Resolutions)
				.SelectMany(x => x.Dependencies)
				.Except(Parameters.Select(x => x.Type));
		}

		public IEnumerable<TypeNode> GetCapableServiceTypes()
		{
			yield return ReturnType;
		}

		public IEnumerable<ISemanticsNode> Traverse()
		{
			yield return Common;
			if (ReturnTypeResolution is not null)
			{
				yield return ReturnTypeResolution;
			}
		}
	}
}
