using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imfact.Annotations;

namespace Imfact.TestSubject.DelegationWithParameter
{
	internal class Service
	{
		public Service(int hoge)
		{
			
		}
	}
	
	internal class FactoryToDelegate : BaseFactory
	{
		private readonly Service _service;

		public FactoryToDelegate(Service service)
		{
			_service = service;
		}

		public override Service ResolveService() => _service;
	}

	[Factory]
	internal abstract partial class BaseFactory
	{
		public abstract Service ResolveService();
	}

	[Factory]
	internal partial class MyFactory4
	{
		public BaseFactory BaseFactory { get; }

		public partial Service ResolveService();
	}
}
