﻿using Imfact.Utilities;
using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Filter;

// コンストラクタを呼ぶ必要があるもの、属性を見る必要があるものは依存関係なので、このステップで収集しておく
// このステップで消化することで後続のステップではAnnotationを見ずに済む
// 基底クラスの情報は「コンストラクタ呼び出し」「リゾルバー呼び出し」両方で使うので後でさらに詳しくする必要がある
internal record FilteredType(INamedTypeSymbol Symbol,
	RecordArray<FilteredMethod> Methods,
	RecordArray<FilteredDependency> BaseTypes,
	RecordArray<FilteredDependency> Resolutions,
	RecordArray<FilteredDependency> Delegations);

internal record FilteredMethod(IMethodSymbol Symbol);

internal record FilteredDependency(INamedTypeSymbol Symbol);