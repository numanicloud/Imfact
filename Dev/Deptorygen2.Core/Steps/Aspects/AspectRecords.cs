using Deptorygen2.Core.Entities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Steps.Aspects
{
	internal enum ResolverKind
	{
		Single, Multi
	}

	internal enum AnnotationKind
	{
		Resolution, Hook, CacheHookPreset, CachePrHookPreset
	}

	internal record ClassAspect(TypeNode Type,
		ClassAspect[] BaseClasses,
		MethodAspect[] Methods,
		PropertyAspect[] Properties);

	internal record MethodAspect(string Name,
		Accessibility Accessibility,
		ResolverKind Kind,
		ReturnTypeAspect ReturnType,
		MethodAttributeAspect[] Attributes,
		ParameterAspect[] Parameters);

	internal record PropertyAspect(TypeNode Type, string Name, MethodAspect[] MethodsInType);

	internal record ParameterAspect(TypeNode Type, string Name);

	internal record MethodAttributeAspect(AnnotationKind Kind,
		TypeNode OwnerReturnType,
		string OwnerName,
		TypeToCreate TypeToCreate);

	internal record ReturnTypeAspect(TypeToCreate Type, bool IsAbstract);

	internal record TypeToCreate(TypeNode Node, TypeNode[] ConstructorArguments);
}
