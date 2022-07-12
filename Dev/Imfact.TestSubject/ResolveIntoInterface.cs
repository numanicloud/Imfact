using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imfact.Annotations;

namespace Imfact.TestSubject.ResolveIntoInterface;

internal class Service
{
	public Service(IResolveIntoInterfaceFactory factory)
	{
	}
}

internal interface IResolveIntoInterfaceFactory
{
}

[Factory]
internal partial class ResolveIntoInterfaceFactory : IResolveIntoInterfaceFactory
{
	public partial Service ResolveService();
}

internal class ResolveIntoInterfaceProgram
{
	public void Main()
	{
		var factory = new ResolveIntoInterfaceFactory();
	}
}