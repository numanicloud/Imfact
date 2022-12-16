using Imfact.Entities;
using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Filter.Wrappers;

internal interface ITypeWrapper
{
	bool IsConstructableClass { get; }
	IEnumerable<IAnnotationWrapper> GetAttributes();
	TypeAnalysis GetTypeAnalysis();
}

internal interface IInterfaceImplementor
{
	IEnumerable<ITypeWrapper> AllInterfaces { get; }
}

internal interface IFactoryClassWrapper : IInterfaceImplementor
{
	IEnumerable<IAnnotationWrapper> GetAttributes();
	TypeAnalysis GetTypeAnalysis();

	IResolverWrapper[] Methods { get; }
	IBaseFactoryWrapper? BaseType { get; }
	IResolutionFactoryWrapper[] Resolutions { get; }
	IDelegationFactoryWrapper[] Delegations { get; }
}

internal interface IResolverWrapper
{
	string Name { get; }
	Accessibility Accessibility { get; }
	IReturnTypeWrapper ReturnType { get; }
	IEnumerable<IAnnotationWrapper> Annotations { get; }
    bool IsIndirectResolver();
}

internal interface IReturnTypeWrapper : ITypeWrapper
{
    IResolutionFactoryWrapper ToResolution();
}

internal interface IAnnotationWrapper
{
	string FullName { get; }
	ITypeWrapper? GetSingleConstructorArgumentAsType();
	ITypeWrapper? GetSingleTypeArgument();
    TypeAnalysis GetTypeAnalysis();
}

internal interface IAttributeWrapper
{
	bool IsInSameModuleWith(ITypeWrapper other);
	bool IsUsedAs(IAnnotationWrapper annotation);
	string GetFullNameSpace();
	string Name { get; }
}

internal interface IBaseFactoryWrapper : ITypeWrapper, IInterfaceImplementor
{
	public IEnumerable<IResolverWrapper> Methods { get; }
	public IBaseFactoryWrapper? BaseType { get; }
}

internal interface IResolutionFactoryWrapper : ITypeWrapper
{
}

internal interface IDelegationFactoryWrapper : ITypeWrapper
{
}