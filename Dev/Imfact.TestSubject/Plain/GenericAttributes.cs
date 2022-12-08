using Imfact.Annotations;

namespace Imfact.TestSubject.GenericAttributes;

internal interface IHoge
{
}

internal class Hoge : IHoge
{
}

[Factory]
internal partial class HogeFactory
{
    [Resolution<Hoge>]
    public partial IHoge ResolveHoge();
}