using Imfact.Annotations;

namespace Imfact.TestSubject.InterfaceDelegation;

public class Service
{
}

[Factory]
public interface IFactory
{
	Service ResolveService();
}

[Factory]
internal partial class InterfaceDelegationFactory
{
	public IFactory BaseFactory { get; }

	public partial Service ResolveService();
}

internal class InterfaceDelegationProgram
{
	public void Main()
	{
		var factory = new InterfaceDelegationFactory(new Factory());
		var service = factory.ResolveService();
	}

	private class Factory : IFactory
	{
		public Service ResolveService()
		{
			throw new System.NotImplementedException();
		}
	}
}