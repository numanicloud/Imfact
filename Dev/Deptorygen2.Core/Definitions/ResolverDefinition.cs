using System;
using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Semanticses;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Definitions
{
	public record ResolverDefinition(Accessibility AccessLevel,
		string Name,
		TypeName ReturnType,
		ResolutionDefinition Resolution,
		ResolverParameterDefinition[] Parameters,
		HookDefinition[] Hooks)
	{
		internal ResolverDefinition(
			ResolverNodeData nodeData,
			ResolutionDefinition resolution,
			ResolverParameterDefinition[] parameters,
			HookDefinition[] hooks)
			: this(nodeData.AccessLevel, nodeData.Name, nodeData.ReturnType,
				resolution, parameters, hooks)
		{
		}

		internal static IEnumerable<ResolverDefinition> Build(FactorySemantics factory,
			Func<ResolverSemantics, ResolverNodeData, ResolverDefinition> childrenBuilder)
		{
			return factory.Resolvers.Select(resolver =>
			{
				var data = new ResolverNodeData(resolver.Accessibility,
					resolver.MethodName,
					resolver.ReturnTypeName);
				return childrenBuilder.Invoke(resolver, data);
			});
		}
	}

	internal record ResolverNodeData(Accessibility AccessLevel, string Name, TypeName ReturnType);
}