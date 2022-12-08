using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imfact.Annotations;

namespace Imfact.TestSubject2.Objects2
{
	interface IPika<T>
	{
	}

	interface IFunya<T>
	{
	}

	class Service
	{
	}

	class ServiceGold
	{
		
	}

	class Impl : IPika<IFunya<Service>>
	{
	}

	class ImplGold : IPika<IFunya<ServiceGold>>
	{
		
	}

	[Factory]
	partial class HogeFactory
	{
		public Service GetService(IPika<IFunya<Service>> input)
		{
			return new Service();
		}
	}

	[Factory]
	internal partial class JoyFactory
	{
		private HogeFactory Hoge { get; }

		public partial Impl GetImpl();

		[Resolution(typeof(ImplGold))]
		public partial IPika<IFunya<ServiceGold>> ResolveGold();

		[Resolution(typeof(Impl))]
		public partial IPika<IFunya<Service>> Resolve();

		public partial Service GetService();
	}
}
