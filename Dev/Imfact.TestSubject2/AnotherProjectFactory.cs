using Imfact.Annotations;

namespace Imfact.TestSubject2.AnotherProjectFactoryTest;

public class Service
{
}

[Factory]
public partial class AnotherProjectFactory
{
	public partial Service ResolveService();
}