using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Deptorygen2.TestSubject.Objects.Sub;
using Imfact.Annotations;

namespace Deptorygen2.TestSubject.Objects
{
	/* Deptorygen2 機能一覧
	 * リゾルバー生成
	 * コレクションリゾルバー生成
	 * 委譲ファクトリー
	 * 基底ファクトリー
	 * パラメータつき解決
	 * フック
	 *		キャッシュ
	 *		解決単位のキャッシュ
	 * Disposable対応
	 * AsyncDisposable対応
	 * アクセシビリティ制御
	 * Resolution属性によるバインド
	 */

	/*
	 * Factory属性のついたクラスはファクトリークラスとなる。
	 * ファクトリークラス内のpartialなメソッドはリゾルバーメソッドとなる。
	 *		そのうち、IEnumerableを返すものはコレクションリゾルバーである。
	 * ファクトリークラス内のプロパティで、その型がファクトリークラスである場合、委譲ファクトリーとなる。
	 * ファクトリークラスの基底クラスで、その型がファクトリークラスである場合、基底ファクトリーとなる。
	 * IHook<T>を実装するクラスはフッククラスである。
	 * キャッシュ属性はフッククラスの糖衣構文。
	 * 
	 */

	/*
	 * 基本的なルール
	 * partialメソッドはリゾルバーとなる。
	 * 基底、委譲であるファクトリーからも、普通の方法でアクセスできるリゾルバーを利用する。
	 * フッククラスの処理を属性で差し込むことができる。
	 * 不足している依存関係はコンストラクタで要求される。
	 * 生成の都合で保持しているIDisposableなものは、Disposeメソッドで破棄できる。
	 * Resolution属性で実際に生成するクラスをバインドできる。
	 */


	class Client
	{
		public Client(Sub.Service service, Sub.Service service2)
		{
		}
	}

	class ServiceGold
	{
		public ServiceGold(Client client)
		{
		}
	}

	interface IHoge
	{

	}

	class Hoge : IHoge
	{
	}

	class Hoge2 : IHoge
	{
		
	}

	public class Resource : IDisposable
	{
		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}

	public class ResourceAsync : IAsyncDisposable
	{
		public ValueTask DisposeAsync()
		{
			throw new NotImplementedException();
		}
	}

	public class Consumer
	{
		public Consumer(Resource r, ResourceAsync ra)
		{
		}
	}


	[Factory]
	public partial class PinkFactory
	{
		protected partial Service ResolveService();
		public partial Consumer ResolveConsumer();
	}

	// 生成されるコンストラクタはinternalであるべきかも。あるいは依存関係の中で最も厳しいアクセシビリティ
	[Factory]
	public partial class YellowFactory : PinkFactory
	{
		internal partial Client ResolveClient();

		internal Client GetClient() => ResolveClient();
	}

	[Factory]
	internal partial class CapturedFactory
	{
		[Hook(typeof(Cache<>))]
		internal partial Client ResolveClient2(Service service);

		internal Service ResolveService() => new Service();
	}
	
	// Next: BaseType, Hook, CollectionResolver
	[Factory]
	internal partial class MyFactory
	{
		private CapturedFactory Captured { get; }

		[Hook(typeof(CachePerResolution<Client>))]
		internal partial Client ResolveClient();

		public partial ServiceGold ResolveServiceGold();

		[Resolution(typeof(Hoge))]
		[CachePerResolution]
		[Cache]
		private partial IHoge ResolveHoge();

		[Cache]
		[Resolution(typeof(Hoge))]
		[Resolution(typeof(Hoge2))]
		public partial IEnumerable<IHoge> ResolveMultiHoge();

	}
	
}

namespace Deptorygen2.TestSubject.Objects.Sub
{
	public class Service
	{
		
	}
}