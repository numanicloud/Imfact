
# 基本的な使い方

## 概要

このセクションでは、ファクトリーを用いてクラスのインスタンスを生成する手順を説明します。

今回生成したいクラス`Client`は、`Service`という別のクラスへ依存しています。
この依存関係を簡潔に記述できるのが Imfact の強みです。

手順の中で、「ファクトリークラス」と「リゾルバーメソッド」について説明します。
特に、どのようなクラスが「ファクトリークラス」に、
どのようなメソッドが「リゾルバーメソッド」に該当するのかについて知ることは重要です。

## Imfactのはたらき

以下の2つのクラスには、ClientクラスからServiceクラスへの依存があります。
実行するとどうなるか……よりも、依存関係そのものに注目してください。

```csharp
namespace Sample
{
    class Client
    {
        public Client(Service service)
        {
        }
    }

    class Service
    {
    }
}
```

Clientクラスのコンストラクタを見ると分かる通り、Clientクラスを生成するためにはServiceクラスも生成する必要があります。この依存関係を解決しつつClientクラスを生成するファクトリーを書いてみましょう。

```csharp
namespace Sample
{
    [Factory]
    partial class MyFactory
    {
        public partial Service GetService();
        public partial Client GetClient();
    }
}
```

ここで `MyFactory.GetClient` を呼び出すと、Clientクラスの新しいインスタンスが生成されます。
もちろん、Clientクラスのコンストラクタに渡されるService型の値は非nullです。

そして、この `MyFactory` クラスが「ファクトリークラス」です。
ファクトリークラスは以下の条件を満たすものです：

- `Imfact.Annotations.Factory` 属性が付与されている
- partialクラスである

また、`GetClient` メソッドは「リゾルバーメソッド」です。
リゾルバーメソッドは以下の条件を満たすものです：

- ファクトリークラスに属している
- partialメソッドであり、実装を持たない

また、Clientクラスを生成するリゾルバーとServiceクラスを生成するリゾルバーを両方書く必要があります。もしServiceクラスを生成するリゾルバーを書かなければ、Clientクラスの依存関係を自動で解決してくれることはありません。具体的にどうなってしまうのかは別のセクションで解説します。

今は、解決してほしい依存先の型に対しても、リゾルバーメソッドを書く必要があるということを覚えておいてください。

## 使用例

さて、せっかくなので実行して動作を確認できるコードを書いてみましょう。

```csharp
using System;

var factory = new MyFactory();
var client = factory.GetClient();
client.Run();

namespace Sample
{
    class Client
    {
        private readonly Service _service;

        public Client(Service service)
        {
            _service = service;
        }

        public void Run()
        {
            _service.Run();
        }
    }

    class Service
    {
        public void Run()
        {
            Console.WriteLine("Its service yay!");
        }
    }

    [Factory]
    partial class MyFactory
    {
        public partial Service GetService();
        public partial Client GetClient();
    }
}
```

実行結果：

```
Its service yay!
```

## その他詳しいこと

`Imfact.Annotations.FactoryAttribute` 属性クラスは、Imfactのソースジェネレータによって自動的にプロジェクトに追加されます。
