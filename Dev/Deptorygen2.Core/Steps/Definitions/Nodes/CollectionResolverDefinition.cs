using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Steps.Definitions
{
	public record CollectionResolverDefinition(Accessibility AccessLevel,
		string MethodName,
		TypeName ReturnType,
		ResolutionDefinition[] Resolutions,
		ResolverParameterDefinition[] Parameters,
		HookDefinition[] Hooks) : IResolverDefinition
	{
		internal CollectionResolverDefinition(CollectionResolverNodeData nodeData,
			ResolutionDefinition[] resolutions,
			ResolverParameterDefinition[] parameters,
			HookDefinition[] hooks)
			: this(nodeData.AccessLevel, nodeData.Name, nodeData.ReturnType,
				resolutions, parameters, hooks)
		{
		}

		internal static IEnumerable<CollectionResolverDefinition> Build(
			FactorySemantics factory,
			ChildrenBuilder childrenBuilder)
		{
			return factory.CollectionResolvers.Select(resolver =>
			{
				var nodeData = new CollectionResolverNodeData(
					resolver.Accessibility, resolver.MethodName, resolver.CollectionType);
				return childrenBuilder.Invoke(resolver, nodeData);
			});
		}

		internal delegate CollectionResolverDefinition ChildrenBuilder(
			CollectionResolverSemantics resolver,
			CollectionResolverNodeData nodeData);
	}

	internal record CollectionResolverNodeData(
		Accessibility AccessLevel,
		string Name,
		TypeName ReturnType);
}