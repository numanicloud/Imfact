using Deptorygen2.Core.Steps.Definitions;
using Deptorygen2.Core.Steps.Semanticses;
using System.Collections.Generic;
using Deptorygen2.Core.Steps.Instantiation.CreationMethods;

namespace Deptorygen2.Core.Steps.Instantiation
{
	/* 自分自身を使って解決しようとするのを避けるためにも、Enumを上手く使って除外設定をしたい。
	 * そうは言っても、「リゾルバー、かつ同じ型」みたいのをうまく表現するのが難しい。
	 * Requestに含めるデータとして、リクエスト元がどのようなInstantiationかを持たせるのはどうか？
	 * SenderType, SenderMethod という名前だとよさそう。
	 *
	 * Resolutionのような最後尾のものは InstantiationMethod.None を送ればよさそう。
	 *
	 * 解決手段を型で表すレイヤーもあると良いのかもしれない？
	 * - コンストラクタで初期化方法の違いを吸収
	 * - その解決手段を使うかどうかを述語メソッドで判断
	 *
	 * Argumentから解決するという手段は他の解決手段と比べて手続きが決まるのが遅れる。
	 * そのため、この解決手段だけはこの型レイヤーに含めないほうがすっきりしそう。
	 * そうなると、この型レイヤーには引数に関する情報を送らなくて良くなる。
	 * 結果として、InstantiationRequest 型のデータは TypeName だけで構わない。
	 *
	 * Semanticsを使うのをやめてDefinitionsを使うようにしたが、どちらかというと
	 * Semanticsを使う方がよいかもしれない。Definitionsにはコード生成に必要な情報だけが
	 * あってほしいが、Injectionにはそうでない情報も要るので、
	 * Definitionステップ内で情報を消費してしまいたい。
	 */

	internal class InstantiationRuleFactory
	{
		public static IEnumerable<IInstantiationCoder> GetCreations(SourceCodeDefinition definition)
		{
			yield return new FactoryItselfCreation(definition);
			yield return new DelegationItselfCreation(definition);
			yield return new DelegatedResolverCreation(definition);
			yield return new DelegatedCollectionResolverCreation(definition);
			yield return new ResolverCreation(definition);
			yield return new CollectionResolverCreation(definition);
			yield return new FieldCreation(definition);
			yield return new ConstructorCreation(definition);
		}
	}
}
