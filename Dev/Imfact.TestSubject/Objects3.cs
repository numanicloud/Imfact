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
		public void Register<TInterface>(TInterface arg)
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
		public partial IHoge ResolveHoge3();

		[Resolution(typeof(Hoge))]
		public partial IHoge ResolveHoge4();

		[Exporter]
		public void Register<TInterface>(Registerer registerer, Func<TInterface> func)
		{
			registerer.Register<TInterface>(func());
		}

		[Exporter] 
		public void Register2<TInterface>(Registerer2 registerer2, Func<TInterface> func)
		{
			registerer2.Register<TInterface>(func());
		}
	}
}

namespace Imfact.Test
{


	public class Registerer2
	{
		public void Register<TInterface>(TInterface arg)
		{
			Console.WriteLine(arg);
		}
	}
}
