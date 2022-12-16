using Imfact.Entities;
using Imfact.Steps.Filter.Wrappers;

namespace Imfact.Steps.Filter;

// コンストラクタを呼ぶ必要があるもの、属性を見る必要があるものは依存関係なので、このステップで収集しておく
// このステップで消化することで後続のステップではAnnotationを見ずに済む
// 基底クラスの情報は「コンストラクタ呼び出し」「リゾルバー呼び出し」両方で使うので後でさらに詳しくする必要がある

// TODO: FilteredデータはSymbolレベルのデータを持たないべき。
// それとは別に、MatchフェーズではSymbolレベルのデータを直接使うべき。

internal class FilteredType
{
	public required TypeAnalysis Type { get; init; }
    public FilteredMethod[] Methods { get; init; } = Array.Empty<FilteredMethod>();
	public FilteredBaseType? BaseFactory { get; init; }
    public FilteredResolution[] ResolutionFactories { get; init; } =
        Array.Empty<FilteredResolution>();
	public FilteredDelegation[] Delegations { get; init; } = Array.Empty<FilteredDelegation>();

	public static FilteredType New(
		TypeAnalysis type,
		FilteredMethod[]? methods = null,
		FilteredBaseType? baseFactory = null,
		FilteredResolution[]? resolutions = null,
		FilteredDelegation[]? delegations = null)
    {
        return new FilteredType
        {
			Type = type,
			Methods = methods ?? Array.Empty<FilteredMethod>(),
			BaseFactory = baseFactory,
			ResolutionFactories = resolutions ?? Array.Empty<FilteredResolution>(),
			Delegations = delegations ?? Array.Empty<FilteredDelegation>()
        };
	}
}

internal class FilteredMethod
{
	public required string Name { get; init; }
	public required TypeAnalysis ReturnType { get; init; }
	public FilteredAttribute[] Attributes { get; init; } = Array.Empty<FilteredAttribute>();
}

internal class FilteredBaseType
{
	public required TypeAnalysis Type { get; init; }
    public FilteredMethod[] Methods { get; init; } = Array.Empty<FilteredMethod>();
	public FilteredBaseType? BaseFactory { get; init; }
}

internal class FilteredResolution
{
	public required TypeAnalysis Type { get; init; }
};

internal class FilteredDelegation
{
	public required TypeAnalysis Type { get; init; }
}

internal record FilteredAttribute(TypeAnalysis Type, AnnotationKind Kind);

internal record ResolutionAttribute(
	TypeAnalysis Type,
	ITypeWrapper Resolution,
	AnnotationKind Kind)
    : FilteredAttribute(Type, Kind);

internal record HookAttribute(
	TypeAnalysis Type,
	ITypeWrapper HookType,
	AnnotationKind Kind)
    : FilteredAttribute(Type, Kind);