using Imfact.Annotations;

namespace Imfact.TestSubject.InterfaceDelegation;

internal class Service
{
}

[Factory]
internal interface IFactory
{
	Service ResolveService();
}

[Factory]
internal partial class InterfaceDelegationFactory
{
	public IFactory BaseFactory { get; }

	public partial Service ResolveService();
}