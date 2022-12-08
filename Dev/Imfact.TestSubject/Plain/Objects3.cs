using System;
using Imfact.Annotations;
using Imfact.Test;

namespace Imfact.TestSubject.Objects3
{
	public interface IHoge
	{
	}

	public class Hoge : IHoge
	{
	}

	public class Fuga
	{
		public Fuga(IHoge hoge)
		{
		}
	}

	public class Piyo
	{
		public Piyo(Fuga fuga)
		{
		}
	}

	internal class Registerer
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
	}

	[Factory]
	internal partial class Factory1
	{
		private Factory3 Factory3 { get; }
		public partial Fuga ResolveFuga();
	}

	[Factory]
	internal partial class Factory2 : Factory1
	{
		public partial Piyo ResolvePiyo();
	}
}

namespace Imfact.Test
{


	internal class Registerer2
	{
		public void Register<TInterface>(TInterface arg)
		{
			Console.WriteLine(arg);
		}
	}
}
