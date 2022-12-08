using Imfact.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imfact.TestSubject.GenericAttributes;

internal interface IHoge
{
}

internal class Hoge
{
}

[Factory]
internal partial class HogeFactory
{
    [Resolution<Hoge>]
    public partial IHoge ResolveHoge();
}