using Imfact.Entities;
using Imfact.Steps.Filter.Wrappers;

namespace Imfact.Steps.Filter;

// コンストラクタを呼ぶ必要があるもの、属性を見る必要があるものは依存関係なので、このステップで収集しておく
// このステップで消化することで後続のステップではAnnotationを見ずに済む
// 基底クラスの情報は「コンストラクタ呼び出し」「リゾルバー呼び出し」両方で使うので後でさらに詳しくする必要がある
internal record FilteredType(IFactoryClassWrapper Symbol,
    FilteredMethod[] Methods,
    FilteredBaseType? BaseFactory,
    FilteredResolution[] ResolutionFactories,
    FilteredDelegation[] Delegations);

internal record FilteredMethod(IResolverWrapper Symbol,
    IReturnTypeWrapper ReturnType,
    FilteredAttribute[] Attributes);

internal record FilteredBaseType(IBaseFactoryWrapper Wrapper,
	FilteredMethod[] Methods,
	FilteredBaseType? BaseFactory);

internal record FilteredResolution(IReturnTypeWrapper Type);

internal record FilteredDelegation(IDelegationFactoryWrapper Wrapper);

internal record FilteredAttribute(IAnnotationWrapper Wrapper, AnnotationKind Kind);

internal record ResolutionAttribute(
	IAnnotationWrapper Wrapper,
	ITypeWrapper Resolution,
	AnnotationKind Kind)
    : FilteredAttribute(Wrapper, Kind);

internal record HookAttribute(
	IAnnotationWrapper Wrapper,
	ITypeWrapper HookType,
	AnnotationKind Kind)
    : FilteredAttribute(Wrapper, Kind);