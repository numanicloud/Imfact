// ReSharper disable CheckNamespace

using Imfact.Annotations;

namespace Imfact.TestSubject.Executable.ResolveDependency;

internal class Hoge
{
	public Hoge(Fuga fuga)
	{
	}
}

internal class Fuga
{
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
	}
}