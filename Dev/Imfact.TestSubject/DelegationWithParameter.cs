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

		public FactoryToDelegate(Service service) : base(1)
		{
			_service = service;
		}

		public override Service ResolveService() => _service;
	}

	[Factory]
	internal partial class BaseFactory
	{
		public virtual partial Service ResolveService();
	}

	[Factory]
	internal partial class MyFactory4
	{
		public BaseFactory BaseFactory { get; }

		public partial Service ResolveService();
	}
}
