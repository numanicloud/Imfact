using Imfact.Annotations;
// ReSharper disable CheckNamespace

namespace Imfact.TestSubject.Executable.ResolveSingle;

internal class Hoge
{
    public string GetText() => "Hoge";
}

[Factory]
internal partial class Factory
{
    public partial Hoge ResolveHoge();
}

internal class Test : ITest
{
    public void Run()
    {
        var factory = new Factory();
        factory.ResolveHoge().GetText().AssertEquals("Hoge");
    }
}
