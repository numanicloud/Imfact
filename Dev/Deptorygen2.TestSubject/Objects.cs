using System;

namespace Deptorygen2.TestSubject
{
	class Client
	{
		public Client(Sub.Service service)
		{
			
		}
	}

	[Factory]
	public partial class MyFactory
	{
		internal partial Client ResolveClient();
	}
}

namespace Deptorygen2.TestSubject.Sub
{
	class Service
	{
		
	}
}

namespace Deptorygen2.TestSubject
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	sealed class FactoryAttribute : Attribute
	{
		// See the attribute guidelines at 
		//  http://go.microsoft.com/fwlink/?LinkId=85236
		public FactoryAttribute()
		{
		}
	}
}