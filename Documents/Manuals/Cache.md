
# インスタンスのキャッシュ

## 概要

ここでは、ファクトリーで生成したインスタンスをキャッシュしたい場合に使える機能を紹介します。

生成に時間のかかるクラスをキャッシュすればパフォーマンスの問題を軽減できるでしょうし、たくさんのクラスから依存されているようなクラスをキャッシュすることで、いつも同一のインスタンスをクラス群に配布する用途で使うこともできます。

また、メソッドを何度呼び出しても同じインスタンスを返すキャッシュの他にも、依存関係のルートとなるインスタンスの解決が終われば破棄されるような「解決単位のキャッシュ」も使用することができます。

## キャッシュの使い方

以下の2つのクラスには、ClientクラスがServiceクラスに依存しているという関係があります。「基本的な使い方」での状態とほぼ同じですね。

実行するとどうなるか……よりも、依存関係があることと、そして重要なこととして、Serviceクラスを生成する処理は実行に時間がかかることに注目してください。

```csharp
namespace Sample
{
    class Service
    {
        public Service()
        {
            // 何か時間のかかる処理
        }
    }

    class Client
    {
        public Client(Service service)
        {
        }
    }
}
```

ファクトリークラスを今まで通り使っていると、もし`Client`クラスを何度も繰り返し生成したい場合、`Service`クラスもその度に生成されることになり、したがって時間のかかる処理も何度も実行されてしまいます。

これを避けるためにも、`Service`クラスを生成する回数を1回だけにできないでしょうか？そのような場合に使うのがキャッシュ機能です。`Service`クラスのインスタンスをキャッシュする機能を持ったファクトリーは、以下のように書けます。

```csharp
namespace Sample
{
    [Factory]
    partial class MyFactory
    {
        [Cache]
        public partial Service GetService();
        public partial Client GetClient();
    }
}
```

注目すべき点は、`GetService`メソッドに`Cache`属性を付けたところです。これによって、`GetService`メソッドが`Service`クラスのインスタンスを返す際、そのインスタンスをキャッシュするようになります。すなわち、最初の1回目の呼び出しでは新しいインスタンスを生成し、2回目以降の呼び出しでは同じインスタンスを使いまわすのです。

## 解決単位でのキャッシュ

もう少し事情が複雑な場合でのキャッシュについて考えてみましょう。

以下のコードでは、`Client`クラスはコンストラクタを通じて、`Service`クラスのインスタンスを2つ要求しています。

```csharp
namespace Sample
{
    class Service
    {
        public Service()
        {
            // 何か時間のかかる処理
        }
    }

    class Client
    {
        public Client(Service service, Service service)
        {
        }
    }
}
```

この場合、キャッシュ無しならば`Service`のインスタンスは2つ生成され、キャッシュ有りならば`Service`のインスタンスは1つだけ生成されるはずです。

もう少し踏み込んだシナリオを考えてみます。`Client`クラスを2つ生成する場合、キャッシュ無しならば`Service`のインスタンスは4つ生成され、キャッシュ有りならば`Service`のインスタンスは1つだけ生成されます。

||Client x1|Client x2|
|-|-|-|
|キャッシュ無し|Service x2|Service x4|
|キャッシュ有り|Service x1|Service x1|
|???|Service x1|Service x2|

ですがもし、Clientクラスのコンストラクタの引数ふたつに同じインスタンスを指定してほしいが、Clientクラスを生成するたびに新しいServiceインスタンスを生成してほしいとしたら、キャッシュ有りでもキャッシュ無しでも希望を満たすことができません。

実際にはそういった状況は無さそうですが、依存関係が複雑になってくると

// 素直にクラスが4,5個出てくる例を使ってもいいかも

## 解決単位でのキャッシュ

もう少し事情が複雑な場合でのキャッシュについて考えてみましょう。

以下のコードにおいて、`ServiceA`クラスのインスタンスを生成する手順について考えてみます。もちろん、依存関係を全て解決する必要があります。

```csharp
namespace Sample
{
    class ServiceA
    {
        public ServiceA(ServiceB b, ServiceC c)
        {
        }
    }

    class ServiceB
    {
        public ServiceB(ServiceD d)
        {
        }
    }

    class ServiceC
    {
        public ServiceC(ServiceD d)
        {
        }
    }

    class ServiceD
    {
    }
}
```

`ServiceA` は2つのクラス `ServiceB`, `ServiceC`に依存していて、そしてその2つのクラスはどちらも`ServiceD`に依存しています。

以下のようなファクトリーがあれば、依存関係を全て解決することができます。

```csharp
namespace Sample
{
    [Factory]
    partial class MyFactory
    {
        public partial Service GetServiceA();
        public partial Service GetServiceB();
        public partial Service GetServiceC();
        public partial Service GetServiceD();
    }
}
```

ここからが問題です。この依存関係の連鎖を解決していくと、`ServiceD`クラスのインスタンスは2つ生成されることになります。もし`ServiceA`の生成1回につき`ServiceD`クラスのインスタンスを1つしか生成したくない場合、前の節で使った`Cache`を利用するといいかもしれません。

しかし`Cache`を使った場合、`ServiceA`を何度も繰り返し生成するとき、全ての生成において同一の`ServiceD`が使用されます。今回は`ServiceA`の生成1回につき`ServiceD`の生成を1回だけにしたいだけなので、これはやりすぎかもしれません。

こういうとき、`CachePerResolution`属性を使うことができます。

```csharp
namespace Sample
{
    [Factory]
    partial class MyFactory
    {
        public partial ServiceA GetServiceA();
        public partial ServiceB GetServiceB();
        public partial ServiceC GetServiceC();
        [CachePerResolution]
        public partial ServiceD GetServiceD();
    }
}
```

このように定義したファクトリーは、あるメソッドが呼ばれてから最終的なインスタンスが呼び出し元に返されるまで、`ServiceD`のインスタンスを1つまでしか生成しません。たとえば、`GetServiceA`メソッドが呼ばれると、`ServiceA`クラスのインスタンスを生成する処理が内部で行われますが、実際にインスタンスが戻り値として返されるまでのあいだ、`ServiceD`のインスタンスは最大で1つまでしか生成されません。

