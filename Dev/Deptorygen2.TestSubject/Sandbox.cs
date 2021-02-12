using Pika;

namespace Fuwa
{
	public class Client
	{
		public Client(Service service)
		{
		}
	}

	[Factory]
	partial class MyFactory
	{
		internal partial Client ResolveClient();
	}
}

namespace Pika
{
	public class Service
	{
	}
}