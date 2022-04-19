using System;
using Imfact.Annotations;
using Imfact.Test;

namespace Imfact.TestSubject
{
	public interface IHoge
	{
	}

	public class Hoge : IHoge
	{
	}

	public class Registerer
	{
		public void Register<TInterface, TImpl>(TInterface arg)
		{
			Console.WriteLine(arg);
		}
	}

	[Factory]
	internal partial class Factory3
	{
		[Resolution(typeof(Hoge))]
		public partial IHoge ResolveHoge();

		[Resolution(typeof(Hoge))]
		public partial IHoge ResolveHoge2();

		[Resolution(typeof(Hoge))]
		[Cache]
		public partial IHoge ResolveHoge3();

		[Exporter]
		public void Register<TInterface, TImpl>(Registerer registerer, Func<TInterface> func)
		{
			registerer.Register<TInterface, TImpl>(func());
		}

		[Exporter] 
		public void Register2<TInterface, TImpl>(Registerer2 registerer2, Func<TInterface> func)
		{
			registerer2.Register<TInterface, TImpl>(func());
		}
	}
}

namespace Imfact.Test
{
	public class Registerer2
	{
		public void Register<TInterface, TImpl>(TInterface arg)
		{
			Console.WriteLine(arg);
		}
	}
}
