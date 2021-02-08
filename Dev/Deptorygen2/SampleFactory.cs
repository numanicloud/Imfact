using System;
using Deptorygen2.Core;

namespace Deptorygen2
{
	class SampleFactory
	{
		public FactoryClass Create()
		{
			return new FactoryClass("MyFactory",
				new FactoryMethod[]
				{
					new FactoryMethod(AccessLevel.Public,
						"Hoge",
						new TypeInfo("MyDomain", "Service"),
						new ResolutionConstructor(new TypeInfo("MyDomain", "Service"), new string[0]),
						new FactoryMethodParameter[0],
						new []
						{
							new HookAnnotation(new TypeInfo("MyDomain", "LogHook")),
							new HookAnnotation(new TypeInfo("MyDomain", "CacheHook")),
						}),
					new FactoryMethod(AccessLevel.Private,
						"Fuga",
						new TypeInfo("MyDomain", "Client"),
						new ResolutionConstructor(new TypeInfo("MyDomain", "Client"),
							new string[]{ "hoge", "_service" }),
						new FactoryMethodParameter[]
						{
							new FactoryMethodParameter(new TypeInfo("System", "Int32"), "hoge")
						},
						new HookAnnotation[0]),
				}, new DependencyField[]
				{
					new DependencyField(new TypeInfo("MyDomain", "Other"), "_other")
				});
		}
	}

	public interface IHook<T>
	{
		T Hook(Func<T> generation);
	}

	public class ResolutionCache<T> : IHook<T> where T : class
	{
		private T? _cache = null;

		public T Hook(Func<T> generation)
		{
			return _cache ??= generation();
		}
	}

	public interface IHookAttribute
	{
		Type HookType { get; }
	}

	public class CacheHookAttribute : Attribute, IHookAttribute
	{
		public Type HookType => typeof(ResolutionCache<>);
	}
}

namespace MyDomain
{
	public class Service
	{

	}

	public class Client
	{
		public Client(Service service, int hoge)
		{
		}
	}
}