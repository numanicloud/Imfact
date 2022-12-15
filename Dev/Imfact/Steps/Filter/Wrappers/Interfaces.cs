﻿using Imfact.Entities;

namespace Imfact.Steps.Filter.Wrappers;

internal interface ITypeWrapper
{
	bool IsInSameModuleWith(ITypeWrapper other);
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
}

internal interface IResolverWrapper
{
	bool IsIndirectResolver();
}

internal interface IReturnTypeWrapper : ITypeWrapper
{
}

internal interface IAnnotationWrapper
{
	string FullName { get; }
	ITypeWrapper? GetSingleConstructorArgumentAsType();
	ITypeWrapper? GetSingleTypeArgument();
}

internal interface IAttributeWrapper : ITypeWrapper
{
	bool IsUsedAs(IAnnotationWrapper annotation);
	string GetFullNameSpace();
	string Name { get; }
}

internal interface IBaseFactoryWrapper : ITypeWrapper, IInterfaceImplementor
{
}

internal interface IResolutionFactoryWrapper : ITypeWrapper
{
}

internal interface IDelegationFactoryWrapper : ITypeWrapper
{
}