using System;
using System.Collections.Generic;
using Deptorygen.Annotations;
using FigmaAltseed.Converter.Steps;
using FigmaAltseed.Records;
using FigmaSharp.Models;

namespace FigmaAltseed.Converter.Abstraction
{
	/* [Resolution(typeof(SvgStep))] とかは長くてイヤだが、
	 * 将来的に [Resolution<SvgStep>] くらいに縮まりそう。
	 *
	 * ServiceLocator的に使う場合、やっぱり AsTransient はダサいかも。
	 * [Scope(Cached | Transient | Resolution)] とかにしたい。
	 * Resolution とは、一回のResolve呼び出し中に複数回要求されたときにだけ同じインスタンスを返すもの。
	 *
	 * StrongInjectも良さそうだけど、見た目が複雑そうなのがイヤなのと、
	 * 今はパラメータつき解決を受け付けないのが難点。
	 * パラメータつき解決は将来追加されそうだが、なんか実現方法が微妙。
	 *
	 * アクセサのような概念を指定できるようにしたい。
	 * [Private] 生成されるコードでメソッドが private になる。
	 *		そのため、privateなメソッドを取り除いた新たなインターフェースを追加することになる。
	 * [Internal] 生成されるコードでメソッドが public になる。
	 *		ただし、internalアクセサのついたインターフェースを実装する。
	 * この辺のことをやりたければ、partialクラスを使った方がいいかも？
	 *
	 * キャッシュって、Decorationパターンで実現できるんじゃないの？
	 * メソッドのデコレーションだけでは無理。変数を置いておく場所がないから
	 * デコレーションクラスを登録し、それをフィールドに持つように生成すると、
	 * デコレーションクラス内に状態を持つことはできそう
	 * 依存関係解決に関する情報をデコレーションクラスのメソッドに渡してあげるといろいろできそう
	 *
	 * DecorationAttributeを継承した属性のみが使用可能。
	 * DecorationAttributeは、IDecoration<T>インターフェースを実装する値を返すメソッドを持つ。
	 *
	 * インターフェース IA を実装するクラス A があるとき、
	 * IA を解決するメソッドと A を解決するメソッドの両方を書きたいときはどうする？
	 * 今回は partial クラスなので、 IAを解決するメソッドが Aに処理を委譲するようにユーザーが書けばよい。
	 *
	 * クラス自体のアクセサはユーザーが指定できるので、コード生成では何も考えない。
	 *
	 *
	 */

	/* 機能リスト
	 *		partialクラスの実装を作ってくれる
	 *		メソッドのシグネチャがそのまま解決メソッドの特性になる
	 *			アクセサが通常通りの意味を持つ
	 *			戻り値が通常通りの意味を持つ
	 *			引数がコンストラクタ引数と同様の意味を持つ
	 *		[Resolution]属性で具象型を指定できる
	 *			対象の型が IEnumerable<T> ならば、複数指定したときに全て注入される
	 *		定義の継承をサポート。共通したメソッド群を持つファクトリークラスを複数生成できる
	 *		実装の委譲をサポート。共通したメソッドがあれば、委譲先を優先的に使って解決する
	 *			継承は「メソッド群の和集合」が対象、委譲は「メソッド群の積集合」が対象、と考えることもできる
	 *		実装のフックをサポート。特定の属性を付与することで、メソッド呼び出しをフックできる
	 *			委譲を「内側のカスタマイズ」に、フックを「外側のカスタマイズ」に使うこともできる
	 *		キャッシュ方法のバリエーションはフック機能のプリセットとして提供される
	 *		IDisposableなオブジェクトを忘れず破棄。
	 *		解決方法を突き止められなかったオブジェクトは、コンストラクタを通じて要求する
	 */

	interface IResolutionContext
	{
		bool IsTopLevel { get; }
		int ResolutionId { get; }
	}

	class CacheDecoration<T> where T : class
	{
		private T? _cache = null;

		public T OnHook(IResolutionContext context, Func<T> generator)
		{
			return _cache ??= generator();
		}
	}

	class PerResolutionDecoration<T> where T : class
	{
		private int _lastResolutionId = 0;
		private T? _cache = null;

		public T OnHook(IResolutionContext context, Func<T> generator)
		{
			if (_cache is null || _lastResolutionId != context.ResolutionId)
			{
				return _cache = generator();
			}

			return _cache;
		}
	}

	// [Factory]
	partial class HogeFactory
	{
		// private なので、内部での解決にしか使われない
		[Resolution(typeof(RestApiStep))]
		private partial IPipelineStep<FigmaCanvas> ResolveCanvas(StartupOption option);

		// internal なので、パッケージ内でのみ呼べる
		[Resolution(typeof(RestApiStep))]
		// [Decoration(typeof(Cache<>))]
		internal partial IPipelineStep<FigmaCanvas> ResolveCanvas2(StartupOption option);

		// public なので、どこからでも呼べる
		[Resolution(typeof(RestApiStep))]
		public partial IPipelineStep<FigmaCanvas> ResolveCanvas3(StartupOption option);

		[Resolution(typeof(SvgFileInfo))]
		[Resolution(typeof(SvgFileInfo))]
		public partial IEnumerable<SvgFileInfo> ResolveSvg();

		public partial IEnumerable<SvgFileInfo> ResolveSvg()
		{
			return new SvgFileInfo[0];
		}

		// protected なら、派生先から呼べる
		// protected internal なら、派生先、またはパッケージ内から呼べる
		// private protected なら、派生先、かつパッケージ内のみから呼べる

		// Capture, Mixin はどうなる？
		// Captureは値レベルの定義を取り込み、したがってスコープも共有する。委譲に相当する。
		// Mixinは型レベルの定義を取り込み、したがってスコープは共有しない。継承に相当する。
		// 委譲する場合、委譲先のオブジェクトで解決したインスタンスに対して、
		// 主体のオブジェクトのやり方でデコレーションする。


		private partial IPipelineStep<FigmaCanvas> ResolveCanvas(StartupOption option)
		{
			return new RestApiStep(option);
		}

		private IPipelineStep<FigmaCanvas>? ResolveCanvas2Cache = null;

		internal partial IPipelineStep<FigmaCanvas> ResolveCanvas2(StartupOption option)
		{
			return ResolveCanvas2Cache ??= new RestApiStep(option);
		}

		// public なので、パッケージ内でのみ呼べる
		[Resolution(typeof(RestApiStep))]
		public partial IPipelineStep<FigmaCanvas> ResolveCanvas3(StartupOption option)
		{
			return new RestApiStep(option);
		}
	}

	[Factory]
	internal interface IPipelineStepFactory
	{
		[Resolution(typeof(RestApiStep))]
		IPipelineStep<FigmaCanvas> GetCanvasStepAsTransient(StartupOption option);

		[Resolution(typeof(SymbolStep))]
		IPipelineStep<ComponentSymbols> GetSymbolStepAsTransient(FigmaCanvas canvas);

		[Resolution(typeof(RecordStep))]
		IPipelineStep<FigmaEmptyNode> GetRecordStepAsTransient(FigmaCanvas canvas, ComponentSymbols symbols);
		
		[Resolution(typeof(SvgStep))]
		IPipelineStep<IEnumerable<SvgFileInfo>> GetSvgStepAsTransient(FigmaCanvas canvas, ComponentSymbols symbols);
		
		[Resolution(typeof(PngStep))]
		IPipelineStep<IEnumerable<PngFileInfo>> GetPngStepAsTransient(IEnumerable<SvgFileInfo> svg);
		
		[Resolution(typeof(AltTransformStep))]
		IPipelineStep<FigmaEmptyNode> GetAltTransformStepAsTransient(FigmaCanvas canvas, FigmaEmptyNode root);
	}
}
