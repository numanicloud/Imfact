using System.Collections.Generic;
using Deptorygen2.Annotations;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Aspects.Nodes;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record Hook(TypeName HookType, string FieldName) : INamespaceClaimer
	{
		public static Hook? Build(Attribute attribute,
			IAnalysisContext context,
			string methodName)
		{
			if (!attribute.IsHook())
			{
				return null;
			}

			var arg = attribute.Data.ConstructorArguments[0].Value;
			if (arg is not INamedTypeSymbol symbol)
			{
				return null;
			}
			
			if (!symbol.ConstructedFrom.IsImplementing(typeof(IHook<>)))
			{
				return null;
			}

			var name = $"_{methodName}_{symbol.Name}";
			return new Hook(TypeName.FromSymbol(symbol), name);
		}

		public IEnumerable<string> GetRequiredNamespaces()
		{
			yield return HookType.FullNamespace;
		}
	}
}
