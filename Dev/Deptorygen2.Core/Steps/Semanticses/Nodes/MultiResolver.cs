using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using IServiceProvider = Deptorygen2.Core.Interfaces.IServiceProvider;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record MultiResolver(ResolverCommon Common)
		: IServiceConsumer, IServiceProvider, INamespaceClaimer, IResolverSemantics
	{
		public TypeNode ReturnType => Common.ReturnType;
		public string MethodName => Common.MethodName;
		public Parameter[] Parameters => Common.Parameters;
		public Accessibility Accessibility => Common.Accessibility;
		public Resolution[] Resolutions => Common.Resolutions;
		public Hook[] Hooks => Common.Hooks;

		public TypeNode ElementType => ReturnType.TypeArguments[0];

		public IEnumerable<TypeNode> GetRequiredServiceTypes()
		{
			return Resolutions.SelectMany(x => x.Dependencies)
				.Except(Parameters.Select(x => x.Type));
		}

		public IEnumerable<TypeNode> GetCapableServiceTypes()
		{
			yield return ReturnType;
		}

		public IEnumerable<ISemanticsNode> Traverse()
		{
			yield return Common;
		}

		public IEnumerable<string> GetRequiredNamespaces()
		{
			yield return "System.Collections.Generic";
		}
	}
}
