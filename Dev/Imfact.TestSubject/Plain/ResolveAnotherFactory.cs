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

[Factory]
internal interface IAnotherFactory
{
}

internal interface IContext
{
}

[Factory]
internal partial class ResolveAnotherFactory : IResolveAnotherFactory
{
	[Resolution(typeof(AnotherFactory))]
	public partial IAnotherFactory ResolveAnother(IContext context);
}

[Factory]
internal partial class AnotherFactory : IAnotherFactory
{
	public IResolveAnotherFactory Resolver { get; }
	public partial Service ResolveService();
}
