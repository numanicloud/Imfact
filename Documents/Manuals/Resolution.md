
# Resolution属性

以下のような1つのインターフェースと、1つのクラスを例に挙げます。

```csharp
namespace Sample
{
    interface IService
    {
    }

    class Service : IService
    {
    }

    class Client
    {
        public Client(IService service)
        {
        }
    }
}
```

`Client`クラスは`IService`インターフェースに依存しています。これは`IService`の実装である`Service`クラスのインスタンスを、コンストラクタを通じて要求している状況です。

`IService`インターフェースへの依存を解決してほしいが、その際に具体的な型として`Service`を選んでほしい場合は、以下のように`Resolution`属性を使えば目的を達成できます。

```csharp
namespace Sample
{
    partial class MyFactory
    {
        [Resolution(typeof(Service))]
        public partial IService GetService();
        public partial Client GetClient();
    }
}
```

このように書いた`GetService`メソッドは、`IService`インターフェースのインスタンスを要求するクラスが生成されるときに呼ばれて、実装として`Service`クラスのインスタンスを生成します。
