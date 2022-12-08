using Imfact.Annotations;

namespace Imfact.TestSubject.InterfaceDelegation;

[Factory]
public interface IFactory
{
	Service ResolveService(int hoge);
}

[Factory]
public partial class InterfaceDelegationFactoryPartner
{
	public partial Service ResolveService(int hoge);
}