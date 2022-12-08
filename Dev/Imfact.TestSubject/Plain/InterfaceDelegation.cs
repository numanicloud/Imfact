using Imfact.Annotations;

namespace Imfact.TestSubject.InterfaceDelegation;

public class Service
{
	public Service(int x)
	{
	}
}

[Factory]
internal partial class InterfaceDelegationFactory
{
	public IFactory BaseFactory { get; }

	public partial Service ResolveService(int x);
}

internal class InterfaceDelegationProgram
{
	public void Main()
	{
		var factory = new InterfaceDelegationFactory(new Factory());
		var service = factory.ResolveService(9);
	}

	private class Factory : IFactory
	{
		public Service ResolveService(int x)
		{
			throw new System.NotImplementedException();
		}
	}
}