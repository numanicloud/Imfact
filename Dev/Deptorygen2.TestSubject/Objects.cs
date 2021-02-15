using System;
using System.Collections.Generic;
using Deptorygen;
using Deptorygen2.Annotations;

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

	class Hoge2 : IHoge
	{
		
	}

	[Factory]
	internal partial class CapturedFactory
	{
		[Hook(typeof(Cache<>))]
		internal partial Client ResolveClient2();
	}
	
	// Next: BaseType, Hook, CollectionResolver
	[Factory]
	internal partial class MyFactory
	{
		private CapturedFactory Captured { get; }

		[Hook(typeof(Cache<Client>))]
		internal partial Client ResolveClient();

		public partial ServiceGold ResolveServiceGold();

		[Resolution(typeof(Hoge))]
		[CachePerResolution]
		[Cache]
		private partial IHoge ResolveHoge();

		[Cache]
		[Resolution(typeof(Hoge))]
		[Resolution(typeof(Hoge2))]
		public partial IEnumerable<IHoge> ResolveMultiHoge();
	}
}

namespace Deptorygen2.TestSubject.Sub
{
	class Service
	{
		
	}
}