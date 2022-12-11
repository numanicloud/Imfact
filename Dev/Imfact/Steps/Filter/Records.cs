using Imfact.Utilities;
using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Filter;

// コンストラクタを呼ぶ必要があるもの、属性を見る必要があるものは依存関係なので、このステップで収集しておく
// このステップで消化することで後続のステップではAnnotationを見ずに済む
// 基底クラスの情報は「コンストラクタ呼び出し」「リゾルバー呼び出し」両方で使うので後でさらに詳しくする必要がある
internal record FilteredType(INamedTypeSymbol Symbol,
	RecordArray<FilteredMethod> Methods,
	RecordArray<FilteredDependency> BaseFactories,
	RecordArray<FilteredDependency> ResolutionFactories,
	RecordArray<FilteredDelegation> Delegations);

internal record FilteredMethod(IMethodSymbol Symbol, RecordArray<FilteredAttribute> Attributes);

internal record FilteredDependency(INamedTypeSymbol Symbol);

internal record FilteredDelegation(IPropertySymbol Symbol);

internal record FilteredAttribute(AttributeData Data);

internal record ResolutionAttribute(AttributeData Data, INamedTypeSymbol Resolution)
	: FilteredAttribute(Data);

internal record HookAttribute(AttributeData Data, INamedTypeSymbol HookType)
	: FilteredAttribute(Data);