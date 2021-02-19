using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Steps.Aspects.Nodes;
using Attribute = Deptorygen2.Core.Steps.Aspects.Nodes.Attribute;

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

	internal record ClassAspect(ClassAspect[] BaseClasses,
		MethodAspect[] Methods,
		PropertyAspect[] Properties);

	internal record MethodAspect(ResolverKind Kind,
		ReturnTypeAspect ReturnType,
		MethodAttributeAspect[] Attributes,
		ParameterAspect[] Parameters);

	internal record PropertyAspect(MethodAspect[] MethodsInType);

	internal record ParameterAspect(TypeNode Type, string Name);

	internal record MethodAttributeAspect(AnnotationKind Kind,
		TypeNode OwnerReturnType,
		string OwnerName);

	internal record ReturnTypeAspect(TypeNode Type, bool IsAbstract);
}
