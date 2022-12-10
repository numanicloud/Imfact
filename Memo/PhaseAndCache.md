
あるフェーズの結果が前回のイテレーションと同じだった場合、以降のフェーズを全て飛ばして同じ値を使う仕組みになっている。

## 1. Predicateフェーズ

```
SyntaxNode -> FilterdSymbol
```

Predicate でクラス定義ノードだけに絞り込む。
Transform で専用の型 FactoryFullInfo に変換する。

Transform内では以下の判定をする。

- partialクラスだけがファクトリーである。
- partialメソッドだけがリゾルバーである。

## 2. Annotationフェーズ

```
FilterdSymbol -> (FilterdSymbol, AnnotationContext) -> FilterdSymbol
```

Combineでアノテーション情報と合成しておく。
PostTransform でアノテーションの判定をする。

- Factory属性がついているクラスだけがファクトリーである。

## 3. Cacheablityフェーズ

```
Array<FilterdSymbol> -> Array<FilterdSymbol, Array<FilterdSymbol>> -> Array<CacheabilityResult>

CacheabilityResult: Array<Dependency>
Dependency: ConstructorName, Parameters
```

リゾルバーは戻り値にファクトリーを持つ場合があるが、そういったリゾルバーのキャッシュは寿命が短いので他のキャッシュと分ける。
最小で1個の部分に分割する。合計で(1 + <ファクトリーを解決するリゾルバーの数>)個の部分に分割する。

ファクトリーを解決するリゾルバーの情報は `FactoryResolverFullInfo` に分割される。
変更を検知できるように、このクラスは依存先のファクトリー情報を保持する。

コードを順次追加しているあいだは問題ないが、依存関係のある複数のファクトリーが同時に追加されると、コード生成を複数回走らせないといけないかもしれない。その場合キャッシュが効くまで繰り返しアナライザーが走るので生成できないということはない。

### 寿命の短さ？

ファクトリーが5つあった場合、
ファクトリーAの中を変更する場合は必ず更新されるが、
他のファクトリーを変更する場合は更新されない。

つまり、他のファクトリーの構造に依存していないファクトリーが更新される確率は5分の1と見積もれる。
この確率は、他のファクトリーに依存すると増えていく。

## 4. Aspectフェーズ

このフェーズ以降がキャッシュの対象となる。
CacheabilityまでのフェーズはSymbolを含むためキャッシュしない。
Symbolは大きいのでキャッシュするとメモリを圧迫するので。

```
CacheabilityResult -> AspectResult
```

このフェーズ以降で使う情報の全てをここで抽出する。

## 5. Semanticsフェーズ

```
AspectResult -> SemanticsResult
```

生成したいコードの意味を構造として作る。

## 6. Dependencyフェーズ

```
SemanticsResult -> DependencyResult
```

リゾルバーどうしの依存関係を整理する。

## 7. Definitionフェーズ

```
DependencyResult -> DefinitionResult
```

生成するコードをかなり直接的に表現する構造を作る。

## 8. Writingフェーズ

```
DefinitionResult -> (string fileName, string contents)
```

生成するコードを作る。