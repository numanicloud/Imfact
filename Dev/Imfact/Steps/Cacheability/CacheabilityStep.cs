using Imfact.Steps.Filter;

namespace Imfact.Steps.Cacheability;

internal sealed class CacheabilityStep
{
	public CacheabilityResult[] Transform(FilteredType type, CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();

		// TODO: コンストラクタを呼ぶような依存関係ごとにグループ化する
		// ファクトリーの継承をしていればコンストラクタ宣言を隔離する
		// リゾルバーの戻り値がファクトリーならそのリゾルバーを隔離する
		return new[]
		{
			new CacheabilityResult(type, null)
		};
	}
}