using Imfact.Entities;
using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Aspects;

internal enum ResolverKind
{
	Single, Multi
}

internal record ClassAspect(TypeAnalysis Type,
	ClassAspect[] BaseClasses,
	InterfaceAspect[] Implements,
	MethodAspect[] Methods,
	PropertyAspect[] Properties,
	ConstructorAspect? KnownConstructor);

internal record InterfaceAspect(TypeAnalysis Type);

internal record ConstructorAspect(
	Accessibility Accessibility,
	ParameterAspect[] Parameters);

internal record MethodAspect(string Name,
	Accessibility Accessibility,
	ResolverKind Kind,
	ReturnTypeAspect ReturnType,
	MethodAttributeAspect[] Attributes,
	ParameterAspect[] Parameters);

internal record ExporterAspect
	(string Name,
	ParameterAspect[] Parameters,
	TypeAnalysis[] TypeParameters);

internal record PropertyAspect(TypeAnalysis Type, string Name,
	MethodAspect[] MethodsInType, bool IsAutoImplemented);

internal record ParameterAspect(TypeAnalysis Type, string Name);

internal record MethodAttributeAspect(AnnotationKind Kind,
	TypeAnalysis OwnerReturnType,
	string OwnerName,
	TypeToCreate TypeToCreate);

internal record ReturnTypeAspect(TypeToCreate Type, bool IsAbstract);

// TODO: Hook系に対してはフィールドの生成を表すが、Resolutionに対しては解決する型を表すので混乱しそう
// 型を分けたい
internal record TypeToCreate(TypeAnalysis Info, TypeAnalysis[] ConstructorArguments);