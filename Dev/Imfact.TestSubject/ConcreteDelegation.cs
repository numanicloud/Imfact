using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imfact.Annotations;

namespace Imfact.TestSubject.ConcreteDelegation;

internal class Hoge
{
}

[Factory]
internal partial class FactoryToDelegate
{
	public Hoge ResolveHoge() => new Hoge();
}

[Factory]
internal partial class Factory
{
	private FactoryToDelegate Delegation { get; set; }

	public partial Hoge ResolveHoge();
}