using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Syntaxes;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core
{
	internal record CollectionResolverDefinition(Accessibility AccessLevel,
		string Name,
		TypeName ReturnType,
		ResolutionDefinition[] Resolutions,
		ResolverParameterDefinition[] Parameters,
		HookDefinition[] Hooks)
	{
		public CollectionResolverDefinition(CollectionResolverNodeData nodeData,
			ResolutionDefinition[] resolutions,
			ResolverParameterDefinition[] parameters,
			HookDefinition[] hooks)
			: this(nodeData.AccessLevel, nodeData.Name, nodeData.ReturnType,
				resolutions, parameters, hooks)
		{
		}

		public static IEnumerable<CollectionResolverDefinition> Build(
			FactorySyntax factory,
			ChildrenBuilder childrenBuilder)
		{
			return factory.CollectionResolvers.Select(resolver =>
			{
				var nodeData = new CollectionResolverNodeData(
					resolver.Accessibility, resolver.MethodName, resolver.CollectionType);
				return childrenBuilder.Invoke(resolver, nodeData);
			});
		}

		public delegate CollectionResolverDefinition ChildrenBuilder(
			CollectionResolverSyntax resolver,
			CollectionResolverNodeData nodeData);
	}

	public record CollectionResolverNodeData(
		Accessibility AccessLevel,
		string Name,
		TypeName ReturnType);
}