using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imfact.Annotations;

namespace Imfact.TestSubject.ResolveAnotherFactory;

internal class Service
{
	public Service(IContext context)
	{
	}
}

internal class Context
{
}

[Factory]
internal interface IResolveAnotherFactory
{
}

internal interface IContext
{
}

[Factory]
internal partial class AnotherFactory
{
	public IResolveAnotherFactory Delegation { get; }
	public partial Service ResolveService();
}

[Factory]
internal partial class ResolveAnotherFactory : IResolveAnotherFactory
{
	public partial AnotherFactory ResolveAnother(IContext context);
}