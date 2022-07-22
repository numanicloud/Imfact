using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imfact.Annotations;
using Imfact.TestSubject2.AnotherProjectFactoryTest;

namespace Imfact.TestSubject.AnotherProjectFactoryTest
{
	[Factory]
	internal partial class AnotherProjectFactoryRoot
	{
		public AnotherProjectFactory Another { get; }

		public partial Service ResolveService();
	}
}
