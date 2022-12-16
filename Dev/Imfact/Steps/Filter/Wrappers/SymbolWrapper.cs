﻿using Imfact.Entities;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Filter.Wrappers;

internal class TypeSymbolWrapper : ITypeWrapper
{
    public required INamedTypeSymbol Symbol { get; init; }

    public bool IsInSameModuleWith(ITypeWrapper other)
    {
        return other is TypeSymbolWrapper symbolWrapper
            && SymbolEqualityComparer.Default.Equals(Symbol.ContainingModule,
                symbolWrapper.Symbol.ContainingModule);
    }

    public bool IsConstructableClass => Symbol.IsReferenceType
        && !Symbol.IsRecord
        && !Symbol.IsAbstract;

    public IEnumerable<IAnnotationWrapper> GetAttributes()
    {
        return Symbol.GetAttributes()
            .Select(x => new AnnotationSymbolWrapper { Data = x });
    }

    public TypeAnalysis GetTypeAnalysis() => TypeAnalysis.FromSymbol(Symbol);
}

internal class FactorySymbolWrapper : TypeSymbolWrapper, IFactoryClassWrapper
{
    private ITypeWrapper[]? _allInterfaces;

    public required IResolverWrapper[] Methods { get; init; }
    public required IBaseFactoryWrapper? BaseType { get; init; }
    public required IResolutionFactoryWrapper[] Resolutions { get; init; }
    public required IDelegationFactoryWrapper[] Delegations { get; init; }

    public IEnumerable<ITypeWrapper> AllInterfaces =>
        _allInterfaces ??= Symbol.AllInterfaces
            .Select(x => new ReturnTypeSymbolWrapper { Symbol = x })
            .Cast<ITypeWrapper>()
            .ToArray();
}

internal class ResolverSymbolWrapper : IResolverWrapper
{
    public required IMethodSymbol Symbol { get; init; }
    public required string Name { get; init; }
    public required Accessibility Accessibility { get; init; }
    public required IReturnTypeWrapper ReturnType { get; init; }

    public IEnumerable<IAnnotationWrapper> Annotations =>
        Symbol.GetAttributes()
            .Select(x => new AnnotationSymbolWrapper()
            {
                Data = x
            });

    public bool IsIndirectResolver()
    {
        return Symbol.ReturnType.SpecialType == SpecialType.None;
    }
}

internal class ReturnTypeSymbolWrapper : TypeSymbolWrapper, IReturnTypeWrapper
{
    public IResolutionFactoryWrapper ToResolution()
    {
        return new ResolutionSymbolWrapper()
        {
            Symbol = Symbol
        };
    }
}

internal class AnnotationSymbolWrapper : IAnnotationWrapper
{
    public required AttributeData Data { get; init; }

    public string FullName => Data.AttributeClass is { } attr
        ? $"{attr.GetFullNameSpace()}.{attr.Name}"
        : throw new NullReferenceException();

    public ITypeWrapper? GetSingleConstructorArgumentAsType()
    {
        return Data.ConstructorArguments.Length == 1
            && Data.ConstructorArguments[0].Kind == TypedConstantKind.Type
            && Data.ConstructorArguments[0].Value is INamedTypeSymbol type
            ? new TypeSymbolWrapper { Symbol = type }
            : null;
    }

    public ITypeWrapper? GetSingleTypeArgument()
    {
        return Data.AttributeClass is not { } attr
            ? null
            : attr.TypeArguments.Length == 1
                && attr.TypeArguments[0] is INamedTypeSymbol type
                    ? new TypeSymbolWrapper { Symbol = type }
                    : null;
    }

    public TypeAnalysis GetTypeAnalysis()
    {
        return TypeAnalysis.FromSymbol(Data.AttributeClass ?? throw new Exception());
    }
}

internal class AttributeSymbolWrapper : TypeSymbolWrapper, IAttributeWrapper
{
    public bool IsUsedAs(IAnnotationWrapper annotation)
    {
        return annotation is AnnotationSymbolWrapper symbol
            && symbol.Data.AttributeClass is { } other
            && SymbolEqualityComparer.Default.Equals(other.OriginalDefinition, Symbol);
    }

    public string GetFullNameSpace() => Symbol.GetFullNameSpace();

    public string Name => Symbol.Name;
}

internal class BaseFactorySymbolWrapper : TypeSymbolWrapper, IBaseFactoryWrapper
{
    private ITypeWrapper[]? _allInterfaces = null;

    public required IEnumerable<IResolverWrapper> Methods { get; init; }
    public required IBaseFactoryWrapper? BaseType { get; init; }

    public IEnumerable<ITypeWrapper> AllInterfaces =>
        _allInterfaces ??= Symbol.AllInterfaces
            .Select(x => new ReturnTypeSymbolWrapper { Symbol = x })
            .Cast<ITypeWrapper>()
            .ToArray();
}

internal class ResolutionSymbolWrapper : TypeSymbolWrapper, IResolutionFactoryWrapper
{
}

internal class DelegationSymbolWrapper : TypeSymbolWrapper, IDelegationFactoryWrapper
{
}