using System;
using Deptorygen;

namespace Deptorygen
{
	class Placeholder
	{
	}
}

namespace Deptorygen2.TestSubject
{
	class Client
	{
		public Client(Sub.Service service)
		{
			
		}
	}

	class ServiceGold
	{
		public ServiceGold(Client client)
		{
		}
	}

	interface IHoge
	{
	}

	class Hoge : IHoge
	{
	}

	[Factory]
	internal partial class CapturedFactory
	{
		[Hook(typeof(Cache<Client>))]
		internal partial Client ResolveClient2();
	}
	
	// Next: BaseType, Hook, CollectionResolver
	[Factory]
	internal partial class MyFactory
	{
		private CapturedFactory Captured { get; }

		internal partial Client ResolveClient();
		public partial ServiceGold ResolveServiceGold();
		[Resolution(typeof(Hoge))]
		private partial IHoge ResolveHoge();
	}
}

namespace Deptorygen2.TestSubject.Sub
{
	class Service
	{
		
	}
}