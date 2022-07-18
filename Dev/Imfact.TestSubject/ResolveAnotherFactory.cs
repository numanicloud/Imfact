using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imfact.Annotations;

namespace Imfact.TestSubject.ResolveAnotherFactory;

internal class Service
{
	public Service(Context context)
	{
	}
}

internal class Context
{
}

[Factory]
internal partial class AnotherFactory
{
	public partial Service ResolveService();
}

[Factory]
internal partial class ResolveAnotherFactory
{
	public partial AnotherFactory ResolveAnother();
}